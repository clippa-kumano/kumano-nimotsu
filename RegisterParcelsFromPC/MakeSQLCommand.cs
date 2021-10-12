using System;
using System.Collections.Generic;
using System.Text;

namespace RegisterParcelsFromPC
{
    class MakeSQLCommand
    {
        //parcelsテーブル用メンバ変数
        public int parcel_uid, owner_uid, register_staff_uid, release_staff_uid;
        public string owner_room_name, owner_ryosei_name, register_datetime, register_staff_room_name, register_staff_ryosei_name;
        public int placement, fragile, is_released;
        public string release_datetime, release_staff_room_name, release_staff_ryosei_name;
        public int checked_count, is_lost;
        public string lost_datetime;
        public int is_returned;
        public string returned_datetime;
        public int is_operation_error, operation_error_type;
        public string note;
        public int parcels_is_deleted;

        //ryoseiテーブル用メンバ変数
        //uid
        public int ryosei_uid;
        public string room_name, ryosei_name, ryosei_name_kana, ryosei_name_alphabet;
        public int block_id, status, parcels_current_count, parcels_total_count;
        public string slack_id;
        public string parcels_total_waittime;
        public int last_event_id;
        public string last_event_datetime;

        //parcel_eventテーブル用メンバ変数
        public int event_uid;
        public string created_at;
        public int event_type;
        public string event_note;
        public int event_is_deleted;
        //room_name, ryosei_name, note;

        //共通項目
        //uid -> parcel, ryosei, event
        //ryoseiテーブルのuidを他のテーブルのクエリなどから呼び出すときの名前
        //created
        //note -> parcels_note, event_note

        public string forShow_ryosei_table()
        {
            string sql = $@"
SELECT 
[room_name] as '部屋番号'
,[ryosei_name] as '氏名'
,[parcels_current_count] as '荷物数'
, '登録' as '登録'
, '受取' as '受取'
,slack_id
,uid
FROM [parcels].[dbo].[ryosei] 
where block_id='{block_id}'
";

            return sql;

        }
        public string forShow_ryosei_table_night_duty_mode(){
            string sql = $@"
SELECT 
[room_name] as '部屋番号'
,[ryosei_name] as '氏名'
,[parcels_current_count] as '荷物数'
, '登録' as '登録'
, '受取' as '受取'
,slack_id
,uid
FROM [parcels].[dbo].[ryosei] 
where block_id='{block_id}'
and parcels_current_count >= 1
";

            return sql;

        }
        public string forShow_event_table()
        {
            string sql=$@"
SELECT top(50)
case [event_type] when 1 then '登録' when 2 then '受取' when 3 then '削除' when 10 then '当番交代' when 11 then 'モード開始' when 12 then 'モード解除'  else 'その他' end  as '操作種類'
,uid as '#'
,[room_name] as '部屋番号'
,[ryosei_name] as '氏名　　　'
,[created_at] as '操作時刻'
,[note] as '特記事項'
,parcel_uid
,ryosei_uid
,is_finished
FROM parcel_event
where is_deleted=0
order by uid desc
";//parcel_uidとryosei_uidは非表示に設定している
            return sql;

        }
        public string forShow_confirm_msgbox(int uid)
        {
            string sql = $@"
select * from parcels where uid = {uid}
";
            return sql;
        }

        public string toRegister_parcels_table()//
        {
            string sql = $@"
insert into [parcels] 
(owner_uid, owner_room_name,owner_ryosei_name,register_datetime,register_staff_uid, register_staff_room_name,register_staff_ryosei_name,placement) 
values 
(
{owner_uid}
,(select room_name from ryosei where uid={owner_uid})
,(select ryosei_name from ryosei where uid={owner_uid})
,'{register_datetime}'
,{register_staff_uid}
,(select room_name from ryosei where uid={register_staff_uid})
,(select ryosei_name from ryosei where uid={register_staff_uid})
,{placement}
)
";

            return sql;
        }


        public string toRegister_parcelevent_table()
        {
            string sql = $@"
insert into [parcel_event] 
(created_at,event_type,ryosei_uid,room_name,ryosei_name,parcel_uid) 
values 
(
'{register_datetime}'
,{event_type}
,{owner_uid}
,(select room_name from ryosei where uid={owner_uid})
,(select ryosei_name from ryosei where uid={owner_uid})
,(select top(1) uid from parcels order by uid desc) 
)
";

            return sql;

        }

        public string toRegister_ryosei_table()
        {
            string sql = $@"
update ryosei
set 
parcels_current_count=(select parcels_current_count from ryosei where uid={owner_uid})+1
,parcels_total_count=(select parcels_total_count from ryosei where uid={owner_uid})+1
,last_event_datetime='{register_datetime}'
where uid={owner_uid}
";

            return sql;
        }

        public string toRelease_get_all_parcels()
        {

            string sqlstr = $@"
select uid 
from parcels 
where 
owner_uid={owner_uid}
and is_released=0 
and is_deleted=0
";
            return sqlstr;
        }

        public string toRelease_parcels_table(List<int> ParcelID)
        {
            is_released = 1;
            string sql = "";
            foreach (int aParcelID in ParcelID)
            {
                sql += $@"
update [parcels] 
set 
is_released = {is_released}
,release_datetime='{release_datetime}'
,release_staff_uid = {release_staff_uid}
,release_staff_room_name=(select room_name from ryosei where uid={release_staff_uid})
,release_staff_ryosei_name=(select ryosei_name from ryosei where uid={release_staff_uid})
where uid ={aParcelID}
";

            }
            return sql;
        }

        public string toRelease_parcelevent_table(List<int> ParcelID)
        {
            string sql = "";
            event_type = 2;
            foreach (int aParcelID in ParcelID)
            {
                sql += $@"
insert into [parcel_event] 
(created_at,event_type,parcel_uid,ryosei_uid,room_name,ryosei_name) 
values 
(
'{release_datetime}'
,{event_type}
,{aParcelID}
,{owner_uid}
,(select room_name from ryosei where uid={owner_uid})
,(select ryosei_name from ryosei where uid={owner_uid})
)
update [parcel_event] 
set
is_finished=1
where parcel_uid={aParcelID} and event_type=1
";
            }
            return sql;
        }
        public string toRelease_ryosei_table(List<int> ParcelID)
        {
            int parcel_number = ParcelID.Count;
            string sql=$@"
update ryosei 
set parcels_current_count=(select parcels_current_count from ryosei where uid={owner_uid})-{parcel_number}
,last_event_datetime='{release_datetime}'
,parcels_total_waittime='{parcels_total_waittime}'
where uid={owner_uid}
";
            return sql;


        }

        public string toDeleteLogically_event_table()
        {
            string sql = $@"
update parcel_event
set is_deleted=1
where uid={event_uid}

insert into parcel_event
(created_at,event_type,parcel_uid,ryosei_uid,room_name,ryosei_name,target_event_uid,is_finished) 
values 
(
'{created_at}'
,3
,{parcel_uid}
,{owner_uid}
,(select room_name from ryosei where uid={owner_uid})
,(select ryosei_name from ryosei where uid={owner_uid})
,{event_uid}
,1
)
";
            return sql;
        }

        public string toDeleteLogically_parcels_table()
        {
            string sql = $@"
update parcels
set is_deleted=1
where uid={parcel_uid}";
            if (event_type == 2)
            {
                sql = $@"
update [parcels] 
set 
is_released = 0
,release_datetime=NULL
,release_staff_uid = NULL
,release_staff_room_name=NULL
,release_staff_ryosei_name=NULL
where uid ={parcel_uid}
";
            }
            return sql;
        }
        /*        public string toDeleteLogically_parcels_table()
        {
            string sql = $@"
update parcels
set is_deleted=1
where uid={parcel_uid}";
            return sql;
        }*/
        public string toDeleteLogically_ryosei_table()
        {
            string sql = $@"
update ryosei
set 
parcels_total_count=(select parcels_total_count from ryosei where uid={owner_uid})-1
,parcels_current_count=(select parcels_current_count from ryosei where uid={owner_uid})-1
where uid={owner_uid}
";
            if (event_type == 2)
            {
                sql = $@"update ryosei
set 
parcels_total_count=(select parcels_total_count from ryosei where uid={owner_uid})+1
,parcels_current_count=(select parcels_current_count from ryosei where uid={owner_uid})+1
where uid={owner_uid}
";
            }
            return sql;
        }
        /*public string toDeleteLoogically_ryosei_table()
        {
            string sql = $@"
DECLARE @event_type INT =(select event_type from parcel_event where uid = {event_uid})
if @event_type = 1
begin
update ryosei
set 
parcels_total_count=(select parcels_total_count from ryosei where uid={owner_uid})-1
,parcels_current_count=(select parcels_current_count from ryosei where uid={owner_uid})-1
where uid={owner_uid}
end
else if @event_type =2 
begin
update ryosei
set 
parcels_total_count=(select parcels_total_count from ryosei where uid={owner_uid})+1
,parcels_current_count=(select parcels_current_count from ryosei where uid={owner_uid})+1
where uid={owner_uid}
end
";
            return sql;
        }*/

        public string toChangeStaff_event_table()
        {
            event_type = 10;
            string sql = $@"
insert into parcel_event
(created_at,event_type,ryosei_uid,room_name,ryosei_name,parcel_uid) 
values 
(
'{created_at}'
,{event_type}
,{ryosei_uid}
,(select room_name from ryosei where uid={ryosei_uid})
,(select ryosei_name from ryosei where uid={ryosei_uid})
,0
)
";
            return sql;
        }
    
        public string Register_new_ryosei_table()
        {
            string sql = $@"
insert into [ryosei]
(room_name,ryosei_name,ryosei_name_kana,block_id)
values
(
'{room_name}'
,'{ryosei_name}'
,'{ryosei_name_kana}'
,{block_id}
)
";
            return sql;
        }

        public string toChangeMode()
        {
            string sql = $@"
insert into [parcel_event] 
(created_at,event_type) 
values 
(
'{created_at}'
,{event_type} 
)
";
            return sql;
        }

        public string toRegister_slack()
        {
            string sql = $@"
update ryosei
set slack_id = '{slack_id}'
where ryosei_name = '{ryosei_name}'
and room_name = '{room_name}'
";
            return sql;
        }

        public string toPeriodicCheck()
        {
            string sql = $@"
update parcel_event 
set is_finished=1 
where event_type<=2 and is_finished=0 and created_at<'{created_at}'
";
            return sql;
        }
        public string toGetList_PeriodicCheck()
        {
            string sql = $@"
select uid 
from parcel_event
where event_type<=2 and is_finished=0 and created_at<'{created_at}'
";
            return sql;
        }
        

    }
}
