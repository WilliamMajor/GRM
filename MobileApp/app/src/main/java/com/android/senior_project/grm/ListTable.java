package com.android.senior_project.grm;

/**
 * Class to list csv content
 */

import android.content.Intent;
import android.os.Bundle;
import android.os.Parcelable;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ListView;

import java.io.InputStream;
import java.util.List;

public class ListTable  extends AppCompatActivity {

    private ListView listView;
    private ItemArrayAdapter itemArrayAdapter;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.table_layout);
        getIntent();

        // Display back button on action bar
        ActionBar actionBar = getSupportActionBar();
        if (actionBar != null) {
            actionBar.setDisplayHomeAsUpEnabled(true);
        }
        actionBar.setTitle("  GRM");

        // Get csv content and store into a list array
        listView = (ListView) findViewById(R.id.listView);
        itemArrayAdapter = new ItemArrayAdapter(getApplicationContext(), R.layout.item_layout);

        Parcelable state = listView.onSaveInstanceState();
        listView.setAdapter(itemArrayAdapter);
        listView.onRestoreInstanceState(state);

        InputStream inputStream = getResources().openRawResource(R.raw.debug);
        CSVFile csvFile = new CSVFile(inputStream);
        List scoreList = csvFile.read();

        for (int i = 0; i < scoreList.size(); i++) {
            itemArrayAdapter.add((String[])scoreList.get(i));
        }

        // Invokes when item on list is clicked
        listView.setOnItemClickListener(new AdapterView.OnItemClickListener() {

            @Override
            public void onItemClick(AdapterView<?> adapterView, View view, int position, long id) {

                // Disable clickable function for table title
                listView.getChildAt(0).setEnabled(false);

                // If table title is clicked, cancel item click activity
                if (position == 0) {
                    return;
                }

                // Parse in item data to intent
                Intent detailsIntent = new Intent(ListTable.this, ListDetails.class);
                detailsIntent.putExtra("EPC", itemArrayAdapter.getItem(position)[0]);
                detailsIntent.putExtra("TEMP", itemArrayAdapter.getItem(position)[1]);
                detailsIntent.putExtra("CODE", itemArrayAdapter.getItem(position)[2]);
                detailsIntent.putExtra("LATITUDE", itemArrayAdapter.getItem(position)[4]);
                detailsIntent.putExtra("LONGITUDE", itemArrayAdapter.getItem(position)[5]);
                startActivity(detailsIntent);
            }
        });
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
