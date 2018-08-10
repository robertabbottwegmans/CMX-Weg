

The batch files in this folder can be used to reset unit test data
in cases where rqlcmd.exe is available, such as when using our Local Dev environment
or on one of our servers.

TODOs:
    * Fix the path to rqlcmd.exe
    * Fix the site name in the script as needed.

The scripts do not know the path to rqlcmd.exe.

    On Local Dev, rqlcmd.exe should be in C:\RQL\<cluster name>\Platform\rqlcmd.exe

    Update the scripts to reference the path to rqlcmd.exe or copy them 
    to that folder where they can be run by double clicking.

The scripts have a specific site name in them.

    Update the scripts to reference the desired target site.

