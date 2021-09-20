package com.example.top;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ListView;

import java.util.ArrayList;
import java.util.List;

public class TourokuKari extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_touroku_kari);
        ListView lvMenu = findViewById(R.id.lvMenu);
        List<String> menuList = new ArrayList<>();
        menuList.add("A301取手");
        menuList.add("A302天王台");
        menuList.add("A302我孫子");
        menuList.add("A303柏");
        menuList.add("A304松戸");
        menuList.add("A305北千住");
        menuList.add("A305南千住");
        menuList.add("A306日暮里");
        ArrayAdapter<String> adapter = new ArrayAdapter<>(TourokuKari.this,
                android.R.layout.simple_list_item_1, menuList);
        lvMenu.setAdapter(adapter);
        lvMenu.setOnItemClickListener(new ListItemClickListener());
    }

private class ListItemClickListener implements AdapterView.OnItemClickListener{
    @Override
    public void onItemClick(AdapterView<?> parent, View view, int position, long id){
        String item = (String)parent.getItemAtPosition(position);

        MultiSelectDialog dialogFragment = new MultiSelectDialog();
        dialogFragment.show(getSupportFragmentManager(),"MultiSelectDialog");
    }
}
}