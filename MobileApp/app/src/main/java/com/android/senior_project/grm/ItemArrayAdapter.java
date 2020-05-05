package com.android.senior_project.grm;

/**
 * Class to parse csv content into table arrays
 */

import android.content.Context;
import android.graphics.Typeface;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;
import java.util.ArrayList;
import java.util.List;

public class ItemArrayAdapter extends ArrayAdapter<String[]> {
    private List<String[]> scoreList = new ArrayList<String[]>();

    static class ItemViewHolder {
        TextView epc;
        TextView code;
    }

    public ItemArrayAdapter(Context context, int textViewResourceId) {
        super(context, textViewResourceId);
    }

    @Override
    public void add(String[] object) {
        scoreList.add(object);
        super.add(object);
    }

    @Override
    public int getCount() {
        return this.scoreList.size();
    }

    @Override
    public String[] getItem(int index) {
        return this.scoreList.get(index);
    }


    @Override
    public View getView(int position, View convertView, ViewGroup parent) {

        View row = convertView;
        ItemViewHolder viewHolder;
        if (row == null) {
            LayoutInflater inflater = (LayoutInflater) this.getContext().
                    getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            row = inflater.inflate(R.layout.item_layout, parent, false);
            viewHolder = new ItemViewHolder();
            viewHolder.epc = (TextView) row.findViewById(R.id.epc);
            viewHolder.code = (TextView) row.findViewById(R.id.code);
            row.setTag(viewHolder);
        } else {
            viewHolder = (ItemViewHolder)row.getTag();
        }
        String[] stat = getItem(position);
        viewHolder.epc.setText(stat[0]);
        viewHolder.code.setText(stat[2]);

        // Bold and center header
        if (position == 0) {
            viewHolder.epc.setTypeface(null, Typeface.BOLD);
            viewHolder.epc.setGravity(Gravity.CENTER);
            viewHolder.code.setTypeface(null, Typeface.BOLD);
            viewHolder.code.setGravity(Gravity.CENTER);
        } else {
            viewHolder.epc.setTypeface(null, Typeface.NORMAL);
            viewHolder.epc.setGravity(Gravity.LEFT);
            viewHolder.code.setTypeface(null, Typeface.NORMAL);
            viewHolder.code.setGravity(Gravity.LEFT);
        }
        return row;
    }
}

