package com.example.top;

import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;

import java.text.SimpleDateFormat;
import java.util.Date;


public class DatabaseHelper extends SQLiteOpenHelper {
    /**
     * データベースファイル名の定数フィールド。
     */
    private static final String DATABASE_NAME = "nimotsuApp.db";
    /**
     * バージョン情報の定数フィールド。
     */
    private static final int DATABASE_VERSION = 1;

    /**
     * コンストラクタ。
     */
    public DatabaseHelper(Context context) {
        // 親クラスのコンストラクタの呼び出し。
        super(context, DATABASE_NAME, null, DATABASE_VERSION);
    }

    @Override
    public void onCreate(SQLiteDatabase db) {
        //parcelsテーブルの登録
        StringBuilder sb_parcels = new StringBuilder();
        sb_parcels.append("CREATE TABLE parcels (");
        sb_parcels.append(" _id INTEGER PRIMARY KEY,");
        sb_parcels.append(" owner_uid INTEGER,");
        sb_parcels.append(" owner_room_name TEXT,");
        sb_parcels.append(" owner_parcels_name TEXT,");
        sb_parcels.append(" register_datetime TEXT,");
        sb_parcels.append(" register_staff_uid INTEGER,");
        sb_parcels.append(" register_staff_room_name TEXT,");
        sb_parcels.append(" register_staff_parcels_name TEXT,");
        sb_parcels.append(" placement INTEGER default 0,");
        sb_parcels.append(" fragile INTEGER default 0,");
        sb_parcels.append(" is_released INTEGER DEFAULT 0,");
        sb_parcels.append(" release_datetime TEXT,");
        sb_parcels.append(" release_staff_uid INTEGER,");
        sb_parcels.append(" release_staff_room_name TEXT,");
        sb_parcels.append(" release_staff_parcels_name TEXT,");
        sb_parcels.append(" checked_count INTEGER DEFAULT 0,");
        sb_parcels.append(" is_lost INTEGER DEFAULT 0,");
        sb_parcels.append(" lost_datetime TEXT,");
        sb_parcels.append(" is_returned INTEGER DEFAULT 0,");
        sb_parcels.append(" returned_datetime TEXT,");
        sb_parcels.append(" is_operation_error INTEGER DEFAULT 0,");
        sb_parcels.append(" operation_error_type INTEGER,");
        sb_parcels.append(" note TEXT,");
        sb_parcels.append(" is_deleted INTEGER DEFAULT 0,");
        sb_parcels.append(" sharing_status INTEGER default 0");
        sb_parcels.append(");");
        String sql_parcels = sb_parcels.toString();
        db.execSQL(sql_parcels);

        //ryoseiテーブルの登録
        StringBuilder sb_ryosei = new StringBuilder();
        sb_ryosei.append("CREATE TABLE ryosei(");
        sb_ryosei.append(" _id INTEGER PRIMARY KEY,");//
        sb_ryosei.append(" room_name TEXT,");//
        sb_ryosei.append(" ryosei_name TEXT,");//
        sb_ryosei.append(" ryosei_name_kana TEXT,");
        sb_ryosei.append(" ryosei_name_alphabet TEXT,");
        sb_ryosei.append(" block_id INTEGER,");
        sb_ryosei.append(" slack_id TEXT,");
        sb_ryosei.append(" status INTEGER DEFAULT 1,");
        sb_ryosei.append(" parcels_current_count INTEGER DEFAULT 0,");
        sb_ryosei.append(" parcels_total_count INTEGER DEFAULT 0,");
        sb_ryosei.append(" parcels_total_waittime TEXT DEFAULT '0:00:00',");
        sb_ryosei.append(" last_event_id INTEGER,");
        sb_ryosei.append(" last_event_datetime TEXT,");
        sb_ryosei.append("created_at TEXT,");
        sb_ryosei.append("updated_at TEXT,");
        sb_ryosei.append(" sharing_status INTEGER default 0");
        sb_ryosei.append("); ");
        String sql_ryosei = sb_ryosei.toString();
        db.execSQL(sql_ryosei);

        //eventテーブルの登録
        StringBuilder sb_parcel_event = new StringBuilder();
        sb_parcel_event.append("CREATE TABLE parcel_event(");
        sb_parcel_event.append(" _id INTEGER PRIMARY KEY,");
        sb_parcel_event.append(" created_at TEXT,");
        sb_parcel_event.append(" event_type INTEGER,");
        sb_parcel_event.append(" parcel_uid INTEGER,");
        sb_parcel_event.append(" parcel_event_uid INTEGER,");
        sb_parcel_event.append(" room_name TEXT,");
        sb_parcel_event.append(" parcel_event_name TEXT,");
        sb_parcel_event.append("target_event_uid INTEGER,");
        sb_parcel_event.append(" note TEXT,");
        sb_parcel_event.append(" is_finished INTEGER DEFAULT 0,");
        sb_parcel_event.append(" is_deleted INTEGER DEFAULT 0,");
        sb_parcel_event.append(" sharing_status INTEGER default 0");
        sb_parcel_event.append(");");
        String sql_parcel_event = sb_parcel_event.toString();
        db.execSQL(sql_parcel_event);

        //DBの作成時に、ついでにテスト用の寮生データも登録してしまいたいが
        //(insert_text_ryosei関数を呼び出すことで登録できる）
        //DBにテーブルが既に作成されていれば無視され、なければこのonCreate関数が呼ばれてテーブルを作成してくれるという話を読んだが
        //テーブルがないときにだけこのonCreateの分岐に入るのか、それともテーブルがあろうとなかろうとこのonCreateの分岐に入るが既にテーブルがあるためcreate文が無効なだけなのかが判別できなかった
        //そのため、テーブルにデータが登録されているかどうか、で分岐したのちにinsert_text_ryosei関数を呼び出す
        //そうでないと、アプリを開くたびDBのデータが増える気がする
        /*
        int judge=-1;
        Cursor cursor = db.rawQuery("SELECT count(*) FROM ryosei", new String[]{});
        try {
            if (cursor.moveToNext()) {
                judge=cursor.getInt(cursor.getColumnIndex("ryosei"));
            }
        } finally {
            cursor.close();
        }
        if(judge==0){
            insert_test_ryosei(db);
        }
        */
        insert_test_ryosei(db);


    }
    void insert_test_ryosei(SQLiteDatabase db){
        StringBuilder sb_insert_test_ryosei = new StringBuilder();
        sb_insert_test_ryosei.append("insert into ryosei (ryosei_name,block_id,room_name) values ");
        sb_insert_test_ryosei.append("('テスト101',1,'A101'),");
        sb_insert_test_ryosei.append("('テスト102',1,'A101'),");
        sb_insert_test_ryosei.append("('テスト103',1,'A101'),");
        sb_insert_test_ryosei.append("('テスト104',1,'A101'),");
        sb_insert_test_ryosei.append("('テスト105',1,'A102'),");
        sb_insert_test_ryosei.append("('テスト106',1,'A102'),");
        sb_insert_test_ryosei.append("('テスト107',1,'A102'),");
        sb_insert_test_ryosei.append("('テスト108',1,'A102'),");
        sb_insert_test_ryosei.append("('テスト109',1,'A103'),");
        sb_insert_test_ryosei.append("('テスト110',1,'A103'),");
        sb_insert_test_ryosei.append("('テスト111',1,'A103'),");
        sb_insert_test_ryosei.append("('テスト112',1,'A103'),");
        sb_insert_test_ryosei.append("('テスト113',1,'A104'),");
        sb_insert_test_ryosei.append("('テスト114',1,'A104'),");
        sb_insert_test_ryosei.append("('テスト115',1,'A104'),");
        sb_insert_test_ryosei.append("('テスト116',1,'A104'),");
        sb_insert_test_ryosei.append("('テスト117',1,'A105'),");
        sb_insert_test_ryosei.append("('テスト118',1,'A105'),");
        sb_insert_test_ryosei.append("('テスト119',1,'A105'),");
        sb_insert_test_ryosei.append("('テスト120',1,'A105'),");
        sb_insert_test_ryosei.append("('テスト121',1,'A106'),");
        sb_insert_test_ryosei.append("('テスト122',1,'A106'),");
        sb_insert_test_ryosei.append("('テスト123',1,'A106'),");
        sb_insert_test_ryosei.append("('テスト124',1,'A106'),");
        sb_insert_test_ryosei.append("('テスト125',1,'A107'),");
        sb_insert_test_ryosei.append("('テスト126',1,'A107'),");
        sb_insert_test_ryosei.append("('テスト127',1,'A107'),");
        sb_insert_test_ryosei.append("('テスト128',1,'A107'),");
        sb_insert_test_ryosei.append("('テスト129',1,'A108'),");
        sb_insert_test_ryosei.append("('テスト130',1,'A108'),");
        sb_insert_test_ryosei.append("('テスト131',1,'A108'),");
        sb_insert_test_ryosei.append("('テスト132',1,'A108'),");
        sb_insert_test_ryosei.append("('テスト133',1,'A109'),");
        sb_insert_test_ryosei.append("('テスト134',1,'A109'),");
        sb_insert_test_ryosei.append("('テスト135',1,'A109'),");
        sb_insert_test_ryosei.append("('テスト136',1,'A109'),");
        sb_insert_test_ryosei.append("('テスト137',1,'A110'),");
        sb_insert_test_ryosei.append("('テスト138',1,'A110'),");
        sb_insert_test_ryosei.append("('テスト139',1,'A110'),");
        sb_insert_test_ryosei.append("('テスト140',1,'A110'),");
        sb_insert_test_ryosei.append("('テスト141',1,'A111'),");
        sb_insert_test_ryosei.append("('テスト142',1,'A111'),");
        sb_insert_test_ryosei.append("('テスト143',1,'A111'),");
        sb_insert_test_ryosei.append("('テスト144',1,'A111'),");
        sb_insert_test_ryosei.append("('テスト145',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト146',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト147',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト148',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト149',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト150',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト151',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト152',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト153',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト154',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト155',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト156',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト157',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト158',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト159',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト160',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト161',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト162',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト163',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト164',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト165',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト166',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト167',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト168',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト169',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト170',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト171',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト172',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト173',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト174',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト175',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト176',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト177',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト178',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト179',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト180',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト181',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト182',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト183',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト184',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト185',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト186',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト187',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト188',2,'A201'),");
        sb_insert_test_ryosei.append("('テスト189',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト190',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト191',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト192',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト193',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト194',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト195',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト196',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト197',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト198',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト199',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト200',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト201',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト202',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト203',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト204',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト205',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト206',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト207',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト208',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト209',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト210',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト211',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト212',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト213',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト214',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト215',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト216',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト217',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト218',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト219',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト220',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト221',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト222',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト223',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト224',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト225',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト226',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト227',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト228',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト229',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト230',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト231',3,'A301'),");
        sb_insert_test_ryosei.append("('テスト232',3,'A301');");
        String sql_insert_test_ryosei = sb_insert_test_ryosei.toString();
        db.execSQL(sql_insert_test_ryosei);
    }


    public void addParcel (
            SQLiteDatabase db,
            String owner_uid,
            String owner_room,
            String owner_ryosei_name,
            String register_staff_uid,
            String register_staff_room_name,
            String register_staff_ryosei_name,
            int fragile){
        StringBuilder sb_insert_Parcel = new StringBuilder();
        sb_insert_Parcel.append("insert into parcels (" +
                "owner_uid,owner_room_name,owner_parcels_name," +
                "register_datetime," +
                "register_staff_uid,register_staff_room_name,register_staff_parcels_name" +
                ") values (");
        sb_insert_Parcel.append( owner_uid +",");
        sb_insert_Parcel.append( " \"" + owner_room +" \",");
        sb_insert_Parcel.append( " \"" + owner_ryosei_name +"\",");
        // 現在日時情報で初期化されたインスタンスの生成
        Date dateObj = new Date();
        SimpleDateFormat format = new SimpleDateFormat( "yyyy/MM/dd HH:mm:ss" );
        // 日時情報を指定フォーマットの文字列で取得
        String display = format.format( dateObj );
        sb_insert_Parcel.append( " \"" + display +"\",");
        sb_insert_Parcel.append( register_staff_uid +",");
        sb_insert_Parcel.append( " \"" + register_staff_room_name +"\",");
        sb_insert_Parcel.append( " \"" + register_staff_ryosei_name +"\")");

        String sql_insert_test_parcel = sb_insert_Parcel.toString();
        db.execSQL(sql_insert_test_parcel);

    }

    @Override
    public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
    }
}