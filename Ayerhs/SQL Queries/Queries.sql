-- Query to find the names of foreign key constraints on the "tblclients" table "FK_tblclient_roles_tblclients_ClientId"
SELECT conname
FROM pg_constraint
WHERE conrelid = 'public."tblclient_roles"'::regclass
AND contype = 'f';

-- Steps to "Trucate Restart Identity" for table "tblclients"
-- Step 1: Drop foreign key constraint for each table
-- Drop the foreign key constraint
ALTER TABLE public."tblclient_roles" DROP CONSTRAINT "FK_tblclient_roles_tblclients_ClientId";

-- Step 2: "Trucate Restart Identity" for Match table manually

-- Step 3: Recreate or enable the foreign key constraint for proper working of project queries
-- Recreate or enable the foreign key constraint
ALTER TABLE public."tblclient_roles" ADD CONSTRAINT "FK_tblclient_roles_tblclients_ClientId" FOREIGN KEY ("ClientId") REFERENCES public."tblclients" ("Id");


-- Find columns of a table
SELECT column_name
FROM information_schema.columns
WHERE table_name = 'tblclients';


-- Fetch all data from table
SELECT * FROM public."tblclient_roles";












-- Query to find the names of foreign key constraints on the "tblgroups" table "FK_tblgroups_tblpartitions_PartitionId"
SELECT conname
FROM pg_constraint
WHERE conrelid = 'public."tblgroups"'::regclass
AND contype = 'f';

-- Steps to "Trucate Restart Identity" for table "tblpartitions"
-- Step 1: Drop foreign key constraint for each table
-- Drop the foreign key constraint
ALTER TABLE public."tblgroups" DROP CONSTRAINT "FK_tblgroups_tblpartitions_PartitionId";

-- Step 2: "Trucate Restart Identity" for Match table manually

-- Step 3: Recreate or enable the foreign key constraint for proper working of project queries
-- Recreate or enable the foreign key constraint
ALTER TABLE public."tblgroups" ADD CONSTRAINT "FK_tblgroups_tblpartitions_PartitionId" FOREIGN KEY ("PartitionId") REFERENCES public."tblpartitions" ("Id");



SELECT * FROM public."tblpartitions";

SELECT * FROM public."tblgroups";

ALTER TABLE tblgroups
ADD CONSTRAINT "FK_tblgroups_tblpartitions_PartitionId"
FOREIGN KEY ("PartitionId") REFERENCES tblpartitions("Id") ON DELETE CASCADE;

