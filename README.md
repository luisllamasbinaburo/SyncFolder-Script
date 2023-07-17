# SyncFiles

Simple script that compares two folders, and delete files and folders in the second folder that exist in first folder

`syncfolder -toKeep "path_to_keep" -toDelete "path_to_delete" -run`
 
 Parameters:
 - -toKeep : path to folder that will be used to compare
 - -toDelete: path to folder in which delete operation will be perform
 - -run: use to really perform the operation. if not provided, only simulation will be perform, no delete operations
   
