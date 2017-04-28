using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using System;

namespace SliderMenuDroid
{
	[Activity(Label = "SliderMenu", MainLauncher = true, Icon = "@mipmap/icon", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
	public class MainActivity : AppCompatActivity, View.IOnTouchListener
	{
		private bool isOpen, isAllowed, isEnough;
		private LinearLayout llBackMenu;
		private LinearLayout llTopContainer;
		private Button btnMenu;
		private float initX, finalX, maxXOpened, minXOpened = 0;
		private const float limit = 60;
		private const float maxXPercent = 0.60f;
		private const long speedAnimation = 200;
		private const float minScalePercent = 0.8f;
		private const float maxScalePercent = 1.0f;
		private const float minMoveToOpen = 0.20f;

		#region Lifecycle
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.Main);

			InitLocalVarables();
			InitElements();
		}
		
		protected override void OnStart()
		{
			base.OnStart();
			AddEvents();
		}
		
		protected override void OnStop()
		{
			base.OnStop();
			RemoveEvents();
		}
		#endregion


		#region ClassMethods

		private void InitLocalVarables()
		{
			llTopContainer = FindViewById<LinearLayout>(Resource.Id.TopContainer);
			llBackMenu = FindViewById<LinearLayout>(Resource.Id.BackMenu);
			btnMenu = FindViewById<Button>(Resource.Id.btnMenu);
		}

		private void InitElements()
		{
			llTopContainer.SetOnTouchListener(this);
			maxXOpened = (Resources.DisplayMetrics.WidthPixels) * maxXPercent;
		}

		private void OpenMenu()
		{
			llTopContainer.Animate().TranslationX(maxXOpened).ScaleX(minScalePercent).ScaleY(minScalePercent).SetDuration(speedAnimation);
			isOpen = true;
		}

		private void CloseMenu()
		{
			llTopContainer.Animate().TranslationX(minXOpened).ScaleX(maxScalePercent).ScaleY(maxScalePercent).SetDuration(speedAnimation);
			isOpen = false;
		}

		private void OpenCloseMenuManager()
		{
			if (isOpen) {
				CloseMenu();
			} else {
				OpenMenu();
			}
		}
		#endregion

		#region Events

		public void AddEvents()
		{
			btnMenu.Click += BtnMenu_Click;
		}

		public void RemoveEvents()
		{
			btnMenu.Click -= BtnMenu_Click;
		}

		private void BtnMenu_Click(object sender, EventArgs e)
		{
			OpenCloseMenuManager();
		}

		#endregion

		#region Implementation IOnTouchListener
		public bool OnTouch(View v, MotionEvent e)
		{
			switch (e.Action) {
			case MotionEventActions.Up:
				if (isAllowed) {
					if (isEnough) {
						OpenCloseMenuManager();
					} else {
						CloseMenu();
					}
				}
				break;
			case MotionEventActions.Down:
				if (!isOpen && e.RawX < limit) {
					initX = e.RawX;
					isAllowed = true;
				} else if (isOpen) {
					isAllowed = true;
					initX = e.RawX;
				} else {
					isAllowed = false;
				}
				break;
			case MotionEventActions.Move:
				if (isAllowed) {
					finalX = e.RawX;
					var modifier = isOpen ? maxXOpened : minXOpened;
					var moveX = finalX - initX + modifier;
					llTopContainer.SetX(moveX);

					if (!isOpen && moveX > (Resources.DisplayMetrics.WidthPixels * minMoveToOpen)) {
						isEnough = true;
					} else {
						isEnough = false;
					}

				}
				break;
			}
			return true;
		}
		#endregion





	}
}

