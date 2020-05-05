package com.android.senior_project.grm;

/**
 * Class for splash screen to sleep for 2.5 seconds
 */

import android.app.Application;
import android.os.SystemClock;

public class SplashScreenSleep extends Application {

    @Override
    public void onCreate() {
        super.onCreate();
        SystemClock.sleep(2500);
    }
}
