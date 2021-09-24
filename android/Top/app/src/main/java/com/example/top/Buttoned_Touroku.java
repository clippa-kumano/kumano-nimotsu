package com.example.top;

import android.app.Activity;
import android.content.Intent;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteStatement;
import android.os.Bundle;
import android.view.KeyEvent;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListAdapter;
import android.widget.ListView;
import android.widget.SimpleCursorAdapter;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import java.util.ArrayList;
import java.util.List;


public class Buttoned_Touroku extends AppCompatActivity {

    String selectedBlock = "A1";
    //表示するブロック 初期値はA1

    private DatabaseHelper _helper;
    Cursor cursor;

    //ArrayListを用意
    private ArrayList<String > ryosei_block = new ArrayList<>();



    @Override
    protected void onCreate(Bundle savedInstanceState){
        super.onCreate(savedInstanceState);
        setContentView(R.layout.buttoned_touroku_layout);

        //事務当番の名前を受け取る
        Intent intent = getIntent();
        String jimuto_name_Str = intent.getStringExtra("Jimuto_name");
        //事務当番の名前を表示する
        TextView jimuto_name =findViewById(R.id.jimutou_name_show);
        jimuto_name.setText("ただいまの事務当番は "+jimuto_name_Str+" です。");

        selectedBlock = "A1";
        Button buttonA1=(Button)findViewById(R.id.touroku_a1_tab);
        Button buttonA2=(Button)findViewById(R.id.touroku_a2_tab);
        Button buttonA3=(Button)findViewById(R.id.touroku_a3_tab);
        Button buttonA4=(Button)findViewById(R.id.touroku_a4_tab);
        Button buttonB12=(Button)findViewById(R.id.touroku_b12_tab);

        Buttoned_Touroku.BlockSelectListener listener = new Buttoned_Touroku.BlockSelectListener();
        buttonA1.setOnClickListener(listener);
        buttonA2.setOnClickListener(listener);
        buttonA3.setOnClickListener(listener);
        buttonA4.setOnClickListener(listener);
        buttonB12.setOnClickListener(listener);

        Button backbutton =(Button)findViewById(R.id.go_back_button);
        backbutton.setOnClickListener(this::onBackButtonClick);

        ListView listListener = findViewById(R.id.ryousei_list_show);
        listListener.setOnItemClickListener(new ListItemClickListener());

        // DBヘルパーオブジェクトを生成。
        _helper = new com.example.top.DatabaseHelper(Buttoned_Touroku.this);

        SQLiteDatabase db = _helper.getWritableDatabase();
/*
       　this.addRecord("A1","A101","取手");
        this.addRecord("A1","A101","天王台");
        this.addRecord("A1","A102","我孫子");
        this.addRecord("A1","A102","柏");
        this.addRecord("A1","A103","松戸");
        this.addRecord("A1","A103","北千住");
        this.addRecord("A1","A103","南千住");
        this.addRecord("A1","A104","三河島");
        this.addRecord("A1","A104","日暮里");
        this.addRecord("A1","A104","上野");
        this.addRecord("A1","A104","東京");
        this.addRecord("A1","A105","新橋");
        this.addRecord("A1","A105","品川");

        this.addRecord("A2","A201","千葉");
        this.addRecord("A2","A201","稲毛");
        this.addRecord("A2","A202","津田沼");
        this.addRecord("A2","A202","船橋");
        this.addRecord("A2","A203","市川");
        this.addRecord("A2","A203","新小岩");
        this.addRecord("A2","A203","錦糸町");
        this.addRecord("A2","A204","馬喰町");
        this.addRecord("A2","A204","新日本橋");
        this.addRecord("A2","A204","東京");

        this.addRecord("A3","A301","西船橋");
        this.addRecord("A3","A301","船橋法典");
        this.addRecord("A3","A302","市川大野");
        this.addRecord("A3","A302","東松戸");
        this.addRecord("A3","A303","新八柱");
        this.addRecord("A3","A303","新松戸");
        this.addRecord("A3","A303","南流山");


 */
        this.show_ryosei("A1");

    }

    private class BlockSelectListener implements AdapterView.OnClickListener {
        @Override
        public void onClick(View view) {
            switch(view.getId()){
                case R.id.touroku_a1_tab:
                    selectedBlock = "A1";
                    break;
                case R.id.touroku_a2_tab:
                    selectedBlock = "A2";
                    break;
                case R.id.touroku_a3_tab:
                    selectedBlock = "A3";
                    break;
                case R.id.touroku_a4_tab:
                    selectedBlock = "A4";
                    break;
                case R.id.touroku_b12_tab:
                    selectedBlock = "B12";
                    break;
            }

            show_ryosei(selectedBlock);

        }
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        cursor.close();
    }


    public void show_ryosei (String block){
        ryosei_block =new ArrayList<String>();
        // データベースヘルパーオブジェクトからデータベース接続オブジェクトを取得。
        SQLiteDatabase db = _helper.getWritableDatabase();
        // 主キーによる検索SQL文字列の用意。
        String sql = "SELECT heya, ryosei_name FROM ryosei WHERE block = '"+block+"';" ;
        // SQLの実行。
        Cursor cursor = db.rawQuery(sql, null);
        //ブロックの寮生を検索しArrayListに追加
        while(cursor.moveToNext()) {
            // データベースから取得した値を格納する変数の用意。データがなかった時のための初期値も用意。
            String note = "";
            // カラムのインデックス値を取得。
            int dateNote = cursor.getColumnIndex("heya");
            // カラムのインデックス値を元に実際のデータを取得。
            note += cursor.getString(dateNote);
            note += " ";
            int ryouseiNote = cursor.getColumnIndex("ryosei_name");
            note += cursor.getString(ryouseiNote);
            ryosei_block.add(note);
        }
        // リスト項目とListViewを対応付けるArrayAdapterを用意する
        ArrayAdapter adapter = new ArrayAdapter<String>(this, android.R.layout.simple_list_item_1, ryosei_block);

        // ListViewにArrayAdapterを設定する
        ListView listView = (ListView)findViewById(R.id.ryousei_list_show);
        listView.setAdapter(adapter);
    }

    public void addRecord (String block, String heya, String ryousei_name) {
        // データベースヘルパーオブジェクトからデータベース接続オブジェクトを取得。
        SQLiteDatabase db = _helper.getWritableDatabase();
        // データベースヘルパーオブジェクトからデータベース接続オブジェクトを取得。
        String sqlInsert = "INSERT INTO ryosei (block, heya, ryosei_name) VALUES (?, ?, ?)";
        SQLiteStatement stmt = db.compileStatement(sqlInsert);
        // 変数のバイド。
        stmt.bindString(1, block);
        stmt.bindString(2, heya);
        stmt.bindString(3, ryousei_name);
        // インサートSQLの実行。
        stmt.executeInsert();
    }

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        if(keyCode == KeyEvent.KEYCODE_BACK) {
            // 戻るボタンの処理

        }

        return true;
    }

    public void onBackButtonClick(View view){
        finish();
    }

    private class ListItemClickListener implements AdapterView.OnItemClickListener{

        public void onItemClick(AdapterView<?> parent, View view, int position, long id){
            String item = (String)parent.getItemAtPosition(position);
            String show = item + "に荷物登録をしました。（してない）";
            Toast.makeText(Buttoned_Touroku.this, show ,Toast.LENGTH_LONG).show();
        }
    }
}
