
rem delete unit test users
rqlcmd QueryStores cmx-master-dev.compliancemetrix.com.local User.List "User_Name LIKE 'unit.test.user%%'" "" unit_test_users.txt
rqlcmd DelStores -s cmx-master-dev.compliancemetrix.com.local -f unit_test_users.txt
del  unit_test_users.txt

rem delete contacts associated w/ unit test users
rqlcmd QueryStores cmx-master-dev.compliancemetrix.com.local Core_Contact.List "First_Name = 'Unit Test'" "" unit_test_user_contacts.txt
rqlcmd DelStores -s cmx-master-dev.compliancemetrix.com.local -f  unit_test_user_contacts.txt 
del  unit_test_user_contacts.txt

rem delete related data firewall records
rqlcmd QueryStores cmx-master-dev.compliancemetrix.com.local Data_Firewall.List "User_User_Name LIKE 'unit.test.user%%'" "" unit_test_data_firewalls.txt
rqlcmd DelStores -s cmx-master-dev.compliancemetrix.com.local -f  unit_test_data_firewalls.txt
del unit_test_data_firewalls.txt

rem delete test Restaurants 
rqlcmd QueryStores cmx-master-dev.compliancemetrix.com.local Core_Organization_Location.List "Location_ID LIKE 'TestRestaurant%%' OR Location_ID LIKE 'Restaurant%%Test'" "" unit_test_restaurants.txt
rqlcmd DelStores -s cmx-master-dev.compliancemetrix.com.local -f  unit_test_restaurants.txt
del  unit_test_restaurants.txt

rem delete test Retail Locations
rqlcmd QueryStores cmx-master-dev.compliancemetrix.com.local Core_Organization_Location.List "Location_ID LIKE 'TestRetailLocation%%' OR Location_ID LIKE 'RetailLocation%%Test'" "" unit_test_retail_locations.txt
rqlcmd DelStores -s cmx-master-dev.compliancemetrix.com.local -f  unit_test_retail_locations.txt 
del  unit_test_retail_locations.txt

rem delete test Assets
rqlcmd QueryStores cmx-master-dev.compliancemetrix.com.local Core_Asset_Tracking.List "ID LIKE 'TestAsset_1%'" "" unit_test_assets.txt
rqlcmd DelStores -s cmx-master-dev.compliancemetrix.com.local -f  unit_test_assets.txt
del  unit_test_assets.txt



