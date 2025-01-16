using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace DayTodayTransactions
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestStoragePermissions();
            EnsurePermissionsAsync();
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
        public async Task EnsurePermissionsAsync()
        {
            if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Android.Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(Platform.CurrentActivity, new[] { Android.Manifest.Permission.WriteExternalStorage }, 1);
            }
        }
    }
}
