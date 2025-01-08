using Android.App;
using Android.Content.PM;
using Android.OS;

namespace DayTodayTransactions
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            RequestStoragePermissions();
        }

        private void RequestStoragePermissions()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Android.Manifest.Permission.ReadExternalStorage) != Permission.Granted)
                {
                    RequestPermissions(new[] { Android.Manifest.Permission.ReadExternalStorage }, 1);
                }
            }
        }
    }
}
