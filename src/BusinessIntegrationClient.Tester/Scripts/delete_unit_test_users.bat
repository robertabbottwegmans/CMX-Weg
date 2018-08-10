
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
del  unit_test_data_firewalls.txt