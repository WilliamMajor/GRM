package com.android.senior_project.grm;

/**
 * Class to list out item details
 */

import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.TextView;

public class ListDetails  extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.details_layout);
        Intent intent = getIntent();

        // Display back button on action bar
        ActionBar actionBar = getSupportActionBar();
        if (actionBar != null) {
            actionBar.setDisplayHomeAsUpEnabled(true);
        }
        actionBar.setTitle("");


        // Display item details in texts
        TextView textEpc = findViewById(R.id.epc);
        textEpc.setText(intent.getStringExtra("EPC"));
        TextView textTemp = findViewById(R.id.temp);
        textTemp.setText(intent.getStringExtra("TEMP"));
        TextView textCode = findViewById(R.id.code);
        textCode.setText(intent.getStringExtra("CODE"));
        TextView textLat = findViewById(R.id.latitude);
        textLat.setText(intent.getStringExtra("LATITUDE"));
        TextView textLong = findViewById(R.id.longitude);
        textLong.setText(intent.getStringExtra("LONGITUDE"));

    }

    // Functions to return to previous activity when back button is clicked
    public boolean onOptionsItemSelected(MenuItem item){
        switch (item.getItemId()) {
            case android.R.id.home:
                finish();
                return true;
        }
        return super.onOptionsItemSelected(item);
    }

    public boolean onCreateOptionsMenu(Menu menu) {
        return true;
    }

}
