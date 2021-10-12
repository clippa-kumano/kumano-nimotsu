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
import android.widget.TextView;

import org.w3c.dom.Text;

import java.text.SimpleDateFormat;
import java.util.Date;

public class MainActivity extends AppCompatActivity {

    private static final int JIMUTOCHANGE_ACTIVITY = 1001;
    String jimuto_room = "";
    String jimuto_name = "";
    String jimuto_id = "";

    private DatabaseHelper _helper;

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

            Intent intent = new Intent(MainActivity.this, Buttoned_Touroku.class);

            intent.putExtra("Jimuto_id", jimuto_id);
            intent.putExtra("Jimuto_room", jimuto_room);
            intent.putExtra("Jimuto_name", jimuto_name);
            startActivity(intent);
        }
    }

    private class UketoriListener implements  View.OnClickListener {
        @Override
        public void onClick(View view){
            Intent intent = new Intent(MainActivity.this, Nimotsu_show.class);
            startActivity(intent);
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


    private class A101KumanoTourokuListener implements View.OnClickListener {
        @Override
        public void onClick(View view) {

            // データベースヘルパーオブジェクトからデータベース接続オブジェクトを取得。
            SQLiteDatabase db = _helper.getWritableDatabase();

            // 日時情報を指定フォーマットの文字列で取得
            Date dateObj = new Date();
            SimpleDateFormat format = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");
            String date = format.format(dateObj);
            String ryousei = "A101KumanoAjiri";
            String mada = "MadaUketottenai";
            // インサート用SQL文字列の用意。
            //String sqlInsert = "INSERT INTO nimotsu (time, ryosei, done) VALUES (?, ?, ?)";
            String sqlInsert = "INSERT INTO nimotsu VALUES (?, ?, ?)";
            // SQL文字列を元にプリペアドステートメントを取得。
            SQLiteStatement stmt = db.compileStatement(sqlInsert);
            // 変数のバイド。
            stmt.bindString(1, date);
            stmt.bindString(2, ryousei);
            stmt.bindString(3, mada);
            // インサートSQLの実行。
            stmt.executeInsert();

            // 主キーによる検索SQL文字列の用意。
            String sql = "SELECT * FROM nimotsu ";
            // SQLの実行。
            Cursor cursor = db.rawQuery(sql, null);
            // データベースから取得した値を格納する変数の用意。データがなかった時のための初期値も用意。
            String note = "";
            // SQL実行の戻り値であるカーソルオブジェクトをループさせてデータベース内のデータを取得。
            while (cursor.moveToNext()) {
                // カラムのインデックス値を取得。
                int dateNote = cursor.getColumnIndex("time");
                // カラムのインデックス値を元に実際のデータを取得。
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

