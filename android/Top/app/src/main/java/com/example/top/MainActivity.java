package com.example.top;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteStatement;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ListView;
import android.widget.SimpleAdapter;
import android.widget.TextView;
import android.widget.Toast;

import org.w3c.dom.Text;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Objects;



public class MainActivity extends AppCompatActivity {

    private static final int JIMUTOCHANGE_ACTIVITY = 1001;
    String jimuto_room = "";
    String jimuto_name = "";
    String jimuto_id = null;

    private DatabaseHelper _helper;

    private String [] from ={"id","text"};
    private int[] to = {android.R.id.text2,android.R.id.text1};

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);


        ImageButton image_button_touroku = findViewById(R.id.image_button_touroku);
        TourokuListener listener3 = new TourokuListener();
        image_button_touroku.setOnClickListener(listener3);


        Button jimutou_change = findViewById(R.id.jimuto_change_button);
        JimutoChangeListener listener4 = new JimutoChangeListener();
        jimutou_change.setOnClickListener(listener4);

        ImageButton image_button_uketori = findViewById(R.id.image_button_uketori);
        UketoriListener listener5 = new UketoriListener();
        image_button_uketori.setOnClickListener(listener5);

        eventLogshow();
        ListView eventLogshower = findViewById(R.id.event_show);
        eventLogshower.setOnItemClickListener(new EventShowListener());


    }


    public void eventLogshow(){
        List<Map<String, String>> show_eventlist = new ArrayList<>();
        _helper = new com.example.top.DatabaseHelper(MainActivity.this);
        SQLiteDatabase db = _helper.getWritableDatabase();
        String sql = "SELECT uid, created_at, event_type, parcel_uid, room_name, ryosei_name, target_event_uid FROM parcel_event" ;
        Cursor cursor = db.rawQuery(sql,null);
        show_eventlist.clear();
        while(cursor.moveToNext()) {
            Map<String, String> event_raw = new HashMap<>();
            String text = "";
            int index = cursor.getColumnIndex("uid");
            String event_id = String.valueOf(cursor.getInt(index));
            index = cursor.getColumnIndex("created_at");
            text += cursor.getString(index);
            index = cursor.getColumnIndex("event_type");
            int event_type_int = cursor.getInt(index);
            switch (event_type_int) {
                case 1://????????????
                    index = cursor.getColumnIndex("room_name");
                    text += cursor.getString(index);
                    index = cursor.getColumnIndex("ryosei_name");
                    text += cursor.getString(index) + " ???????????????";
                    break;
                case 2://????????????
                    index = cursor.getColumnIndex("room_name");
                    text += cursor.getString(index);
                    index = cursor.getColumnIndex("ryosei_name");
                    text += cursor.getString(index) + " ???????????????";
                    break;
            }
            event_raw.put("id", event_id);
            event_raw.put("text", text);
            show_eventlist.add(event_raw);
        }
        SimpleAdapter adapter = new SimpleAdapter(
                this,
                show_eventlist,
                android.R.layout.simple_list_item_1,
                from,
                to
        );
        ListView listView = (ListView)findViewById(R.id.event_show);
        listView.setAdapter(adapter);
        ListView listListener = findViewById(R.id.event_show);
        listListener.setOnItemClickListener(new EventShowListener());
    }


    public void onReturnJimutoValue(String value, String id) {
        jimuto_id = id;

        String[] newStr = value.split("\\s+");
        jimuto_room = newStr[0];
        jimuto_name = newStr[1];
        TextView jimuto_show = findViewById(R.id.main_jimutou_show);
        jimuto_show.setText(jimuto_room + " " + jimuto_name);
    }





    private class TourokuListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {
            if (jimuto_id == null) {
                String show = "????????????????????????????????????????????????";
                Toast.makeText(MainActivity.this, show ,Toast.LENGTH_LONG).show();
            } else{
                Intent intent = new Intent(MainActivity.this, Buttoned_Touroku.class);
                intent.putExtra("Jimuto_id", jimuto_id);
                intent.putExtra("Jimuto_room", jimuto_room);
                intent.putExtra("Jimuto_name", jimuto_name);
                startActivity(intent);
        }
        }
    }



    private class UketoriListener implements  View.OnClickListener {
        @Override
        public void onClick(View view){
            if (jimuto_id == null) {
                String show = "????????????????????????????????????????????????";
                Toast.makeText(MainActivity.this, show ,Toast.LENGTH_LONG).show();
            } else {
                Intent intent = new Intent(MainActivity.this, Buttoned_Uketori.class);
                intent.putExtra("Jimuto_id", jimuto_id);
                intent.putExtra("Jimuto_room", jimuto_room);
                intent.putExtra("Jimuto_name", jimuto_name);
                startActivity(intent);
            }
        }

    }


    private class JimutoChangeListener implements AdapterView.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent jimuto_intent = new Intent(MainActivity.this, Jimuto_Change.class);
            jimuto_intent.putExtra("Jimuto_name",jimuto_room + " " + jimuto_name);
            jimuto_intent.putExtra("Jimuto_id",jimuto_id);
            startActivityForResult(jimuto_intent,JIMUTOCHANGE_ACTIVITY);
        }
    }

    private class EventShowListener implements AdapterView.OnItemClickListener{
        public void onItemClick(AdapterView<?> parent, View view, int position, long id){
            String event_id;
            String created_at;
            String event_type = null;
            String parcel_uid;
            String room_name;
            String ryosei_name;
            String target_event_uid;
            String is_finished;
            Map<String ,String> item = (Map)parent.getItemAtPosition(position);
            //TextView configshow = findViewById(R.id.showText);
            //configshow.setText(item.get("id"));
            item.get("id");
            _helper = new com.example.top.DatabaseHelper(MainActivity.this);
            SQLiteDatabase db = _helper.getWritableDatabase();
            String sql = "SELECT uid, created_at, event_type, parcel_uid, room_name, ryosei_name, target_event_uid FROM parcel_event WHERE uid = "+
                    item.get("id");
            Cursor cursor = db.rawQuery(sql,null);
            while(cursor.moveToNext()) {
                int index = cursor.getColumnIndex("uid");
                event_id = String.valueOf(cursor.getInt(index));
                index = cursor.getColumnIndex("created_at");
                created_at = cursor.getString(index);
                index = cursor.getColumnIndex("event_type");
                event_type = String.valueOf(cursor.getInt(index));
                index = cursor.getColumnIndex("parcel_uid");
                parcel_uid = String.valueOf(cursor.getInt(index));
                index = cursor.getColumnIndex("room_name");
                room_name = cursor.getString(index);
                index = cursor.getColumnIndex("ryosei_name");
                ryosei_name = cursor.getString(index);
                index = cursor.getColumnIndex("target_event_uid");
                target_event_uid = String.valueOf(cursor.getInt(index));
            }
            switch (event_type){
                case "1":

                    break;
                case "2":

                    break;
                default:
            }


        }

    }




    private class A101KumanoTourokuListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {

            // ????????????????????????????????????????????????????????????????????????????????????????????????????????????
            SQLiteDatabase db = _helper.getWritableDatabase();

            // ????????????????????????????????????????????????????????????
            Date dateObj = new Date();
            SimpleDateFormat format = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");
            String date = format.format(dateObj);
            String ryousei = "A101KumanoAjiri";
            String mada = "MadaUketottenai";
            // ??????????????????SQL?????????????????????
            //String sqlInsert = "INSERT INTO nimotsu (time, ryosei, done) VALUES (?, ?, ?)";
            String sqlInsert = "INSERT INTO nimotsu VALUES (?, ?, ?)";
            // SQL??????????????????????????????????????????????????????????????????
            SQLiteStatement stmt = db.compileStatement(sqlInsert);
            // ?????????????????????
            stmt.bindString(1, date);
            stmt.bindString(2, ryousei);
            stmt.bindString(3, mada);
            // ???????????????SQL????????????
            stmt.executeInsert();

            // ????????????????????????SQL?????????????????????
            String sql = "SELECT * FROM nimotsu ";
            // SQL????????????
            Cursor cursor = db.rawQuery(sql, null);
            // ????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
            String note = "";
            // SQL???????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
            while (cursor.moveToNext()) {
                // ?????????????????????????????????????????????
                int dateNote = cursor.getColumnIndex("time");
                // ????????????????????????????????????????????????????????????????????????
                note += cursor.getString(dateNote);
                int ryouseiNote = cursor.getColumnIndex("ryosei");
                note += cursor.getString(ryouseiNote);
                int ryouseiStatus = cursor.getColumnIndex("done");
                note += cursor.getString(ryouseiStatus);
                note += "\n";


            }
        }
    }


    protected void onActivityResult(int requestCode, int resultCode, Intent intent) {
        super.onActivityResult(requestCode, resultCode, intent);
            switch (requestCode) {
                case JIMUTOCHANGE_ACTIVITY:
                    jimuto_id = intent.getStringExtra("Jimuto_id");
                    String[] newStr = intent.getStringExtra("Jimuto_room_name").split("\\s+");
                    jimuto_room = newStr[0];
                    jimuto_name = newStr[1];
                    TextView jimuto_show = findViewById(R.id.main_jimutou_show);
                    jimuto_show.setText(jimuto_room + " " + jimuto_name);
                default:
            }
        }
    }

