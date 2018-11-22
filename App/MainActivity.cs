using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Com.Cooltechworks.Creditcarddesign;
using System;
using Android.Content;

namespace App
{
    [Activity(Label = "App", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        private int CREATE_NEW_CARD = 0;

        private LinearLayout cardContainer;
        private Button addCardButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            Init();
            SubscribeUi();
        }

        void Init()
        {
            addCardButton = FindViewById<Button>(Resource.Id.add_card);
            cardContainer = FindViewById<LinearLayout>(Resource.Id.card_container);
            populate();
        }

        private void populate()
        {
            var sampleCreditCardView = new CreditCardView(this);

            var name = "Glarence Zhao";
            var cvv = "420";
            var expiry = "01/18";
            var cardNumber = "4242424242424242";

            sampleCreditCardView.CVV = cvv;
            sampleCreditCardView.CardHolderName = name;
            sampleCreditCardView.SetCardExpiry(expiry);
            sampleCreditCardView.CardNumber = cardNumber;

            cardContainer.AddView(sampleCreditCardView);
            int index = cardContainer.ChildCount - 1;
            addCardListener(index, sampleCreditCardView);
        }

        public class ClickListener : Java.Lang.Object, View.IOnClickListener
        {
            public Action<View> Click { get; set; }
            public void OnClick(View v) => Click?.Invoke(v);
        }

        private void SubscribeUi()
        {
            addCardButton.SetOnClickListener(new ClickListener
            {
                Click = v =>
                {
                    Intent intent = new Intent(this, typeof(CardEditActivity));
                    StartActivityForResult(intent, CREATE_NEW_CARD);
                }
            });
        }

        private void addCardListener(int index, CreditCardView creditCardView)
        {
            creditCardView.SetOnClickListener(new ClickListener
            {
                Click = v =>
                {
                    var cv = v as CreditCardView;
                    String cardNumber = cv.CardNumber;
                    String expiry = cv.Expiry;
                    String cardHolderName = cv.CardHolderName;
                    String cvv = cv.CVV;

                    Intent intent = new Intent(this, typeof(CardEditActivity));
                    intent.PutExtra(CreditCardUtils.ExtraCardHolderName, cardHolderName);
                    intent.PutExtra(CreditCardUtils.ExtraCardNumber, cardNumber);
                    intent.PutExtra(CreditCardUtils.ExtraCardExpiry, expiry);
                    intent.PutExtra(CreditCardUtils.ExtraCardShowCardSide, CreditCardUtils.CardSideBack);
                    intent.PutExtra(CreditCardUtils.ExtraValidateExpiryDate, false);

                    // start at the CVV activity to edit it as it is not being passed
                    intent.PutExtra(CreditCardUtils.ExtraEntryStartPage, CreditCardUtils.CardCvvPage);
                    StartActivityForResult(intent, index);
                }
            });
        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok)
            {
                //            Debug.printToast("Result Code is OK", getApplicationContext());

                String name = data.GetStringExtra(CreditCardUtils.ExtraCardHolderName);
                String cardNumber = data.GetStringExtra(CreditCardUtils.ExtraCardNumber);
                String expiry = data.GetStringExtra(CreditCardUtils.ExtraCardExpiry);
                String cvv = data.GetStringExtra(CreditCardUtils.ExtraCardCvv);

                if (requestCode == CREATE_NEW_CARD)
                {
                    CreditCardView creditCardView = new CreditCardView(this)
                    {
                        CVV = cvv,
                        CardHolderName = name,
                        CardNumber = cardNumber
                    };
                    creditCardView.SetCardExpiry(expiry);


                    cardContainer.AddView(creditCardView);
                    int index = cardContainer.ChildCount - 1;
                    addCardListener(index, creditCardView);
                }
                else
                {
                    CreditCardView creditCardView = cardContainer.GetChildAt(requestCode) as CreditCardView;

                    creditCardView.SetCardExpiry(expiry);
                    creditCardView.CardNumber = cardNumber;
                    creditCardView.CardHolderName = name;
                    creditCardView.CVV = cvv;
                }
            }
        }
    }
}