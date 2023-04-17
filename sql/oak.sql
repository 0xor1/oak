DROP DATABASE IF EXISTS Oak;
CREATE DATABASE Oak
CHARACTER SET = 'utf8mb4'
COLLATE = 'utf8mb4_general_ci';
USE Oak;

DROP TABLE IF EXISTS Auths;
CREATE TABLE Auths (
    Id CHAR(22) NOT NULL,
    Email VARCHAR(250) NOT NULL,
    LastSignedInOn DATETIME(3) NOT NULL,
    LastSignInAttemptOn DATETIME(3) NOT NULL,
    ActivatedOn DATETIME(3) NOT NULL,
    NewEmail VARCHAR(250) NULL,
    VerifyEmailCodeCreatedOn DATETIME(3) NOT NULL,
    VerifyEmailCode VARCHAR(50) NOT NULL,
    ResetPwdCodeCreatedOn DATETIME(3) NOT NULL,
    ResetPwdCode VARCHAR(50) NOT NULL,
    LoginCodeCreatedOn DATETIME(3) NOT NULL,
    LoginCode VARCHAR(50) NOT NULL,
    Use2FA       BOOLEAN NOT NULL,
    Lang VARCHAR(7) NOT NULL,
    DateFmt VARCHAR(20) NOT NULL,
    TimeFmt VARCHAR(10) NOT NULL,
    PwdVersion INT NOT NULL,
    PwdSalt    VARBINARY(16) NOT NULL,
    PwdHash    VARBINARY(32) NOT NULL,
    PwdIters   INT NOT NULL,
    PRIMARY KEY Id (Id),
    UNIQUE INDEX Email (Email),
    UNIQUE INDEX NewEmail (NewEmail),
    INDEX(ActivatedOn, VerifyEmailCodeCreatedOn)
);

# cleanup old registrations that have not been activated in a week
SET GLOBAL event_scheduler=ON;
DROP EVENT IF EXISTS authRegistrationCleanup;
DROP EVENT IF EXISTS AuthRegistrationCleanup;
CREATE EVENT AuthRegistrationCleanup
ON SCHEDULE EVERY 24 HOUR
STARTS CURRENT_TIMESTAMP + INTERVAL 1 HOUR
DO DELETE FROM Auths WHERE ActivatedOn=CAST('0001-01-01 00:00:00.000' AS DATETIME(3)) AND VerifyEmailCodeCreatedOn < DATE_SUB(NOW(), INTERVAL 7 DAY);

#BIGINT UNSIGNED time values are all in units of minutes
#BIGINT UNSIGNED fileSize values are all in units of bytes
## abbreviations used est=estimate inc=incurred sub=subtask(s) prev=previous sib=siblings and N suffix means count

DROP TABLE IF EXISTS Orgs;
CREATE TABLE Orgs(
    Id VARCHAR(22) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    CreatedOn DATETIME(3) NOT NULL,
    PRIMARY KEY (Id)
);

DROP TABLE IF EXISTS OrgMembers;
CREATE TABLE OrgMembers(
    Org VARCHAR(22) NOT NULL,
    Member VARCHAR(22) NOT NULL,
    IsActive BOOL NOT NULL DEFAULT 1,
    Name VARCHAR(250) NOT NULL,
    Role VARCHAR(25) NOT NULL, # 'owner', 'admin', 'write_all_projects', 'read_all_projects', 'per_project'
    PRIMARY KEY (Org, Member),
    UNIQUE INDEX (Org, IsActive, Role, Name, Member),
    UNIQUE INDEX (Member, IsActive, Org),
    UNIQUE INDEX(Org, IsActive, Name, Role, Member)
);

DROP TABLE IF EXISTS ProjectLocks;
CREATE TABLE ProjectLocks(
    Org VARCHAR(22) NOT NULL,
    Project VARCHAR(22) NOT NULL,
    PRIMARY KEY(Org, Project)
);

DROP TABLE IF EXISTS Projects;
CREATE TABLE Projects(
    Org VARCHAR(22) NOT NULL,
    Id VARCHAR(22) NOT NULL,
    IsArchived BOOL NOT NULL,
    IsPublic BOOL NOT NULL,
    Name VARCHAR(250) NOT NULL,
    CreatedOn DATETIME(3) NOT NULL,
    CurrencySymbol VARCHAR(5) NOT NULL,
    CurrencyCode VARCHAR(5) NOT NULL,
    HoursPerDay TINYINT UNSIGNED NULL,
    DaysPerWeek TINYINT UNSIGNED NULL,
    StartOn DATETIME(3) NULL,
    EndOn DATETIME(3) NULL,
    FileLimit BIGINT UNSIGNED NOT NULL,
    PRIMARY KEY (Org, Id),
    UNIQUE INDEX(id),
    UNIQUE INDEX(Org, IsArchived, IsPublic, Name, CreatedOn, Id),
    UNIQUE INDEX(Org, IsArchived, IsPublic, CreatedOn, Name, Id),
    UNIQUE INDEX(Org, IsArchived, IsPublic, StartOn, Name, Id),
    UNIQUE INDEX(Org, IsArchived, IsPublic, EndOn, Name, Id),
    INDEX(IsPublic, CreatedOn)
);

DROP TABLE IF EXISTS ProjectMembers;
CREATE TABLE ProjectMembers(
    Org VARCHAR(22) NOT NULL,
    Project VARCHAR(22) NOT NULL,
    Member VARCHAR(22) NOT NULL,
    Role VARCHAR(25) NOT NULL, # 'admin', 'writer', 'reader'
    PRIMARY KEY (Org, Project, Member),
    UNIQUE INDEX (Org, Project, Role, Member),
    UNIQUE INDEX (Member, Org, Project)
);

DROP TABLE IF EXISTS Activities;
CREATE TABLE Activities(
    Org VARCHAR(22) NOT NULL,
    Project VARCHAR(22) NOT NULL,
    Task VARCHAR(22) NULL,
    OccurredOn DATETIME(3) NOT NULL,
    Member VARCHAR(22) NOT NULL,
    Item VARCHAR(22) NOT NULL,
    ItemType VARCHAR(50) NOT NULL,
    TaskDeleted BOOL NOT NULL,
    ItemDeleted BOOL NOT NULL,
    Action VARCHAR(50) NOT NULL,
    TaskName VARCHAR(250) NULL,
    ItemName VARCHAR(250) NULL,
    ExtraInfo VARCHAR(10000) NULL,
    PRIMARY KEY (Org, Project, OccurredOn, Item, Member),
    UNIQUE INDEX (Org, Project, ItemDeleted, OccurredOn, Item, Member),
    UNIQUE INDEX (Org, Project, item, OccurredOn, Member),
    UNIQUE INDEX (Org, Project, task, Item, OccurredOn, Member),
    UNIQUE INDEX (Org, Project, Member, OccurredOn, Item),
    UNIQUE INDEX (Org, Project, Member, ItemDeleted, OccurredOn, Item)
);

DROP TABLE IF EXISTS Tasks;
CREATE TABLE Tasks(
    Org VARCHAR(22) NOT NULL,
    Project VARCHAR(22) NOT NULL,
    Id VARCHAR(22) NOT NULL,
    Parent VARCHAR(22) NULL,
    FirstChild VARCHAR(22) NULL,
    NextSib VARCHAR(22) NULL,
    Member VARCHAR(22) NULL,
    Name VARCHAR(250) NOT NULL,
    Description VARCHAR(1250) NOT NULL,
    CreatedBy VARCHAR(22) NOT NULL,
    CreatedOn DATETIME(3) NOT NULL,
    TimeEst BIGINT UNSIGNED NOT NULL,
    TimeInc BIGINT UNSIGNED NOT NULL,
    TimeSubMin BIGINT UNSIGNED NOT NULL,
    TimeSubEst BIGINT UNSIGNED NOT NULL,
    TimeSubInc BIGINT UNSIGNED NOT NULL,
    CostEst BIGINT UNSIGNED NOT NULL,
    CostInc BIGINT UNSIGNED NOT NULL,
    CostSubEst BIGINT UNSIGNED NOT NULL,
    CostSubInc BIGINT UNSIGNED NOT NULL,
    FileN BIGINT UNSIGNED NOT NULL,
    FileSize BIGINT UNSIGNED NOT NULL,
    FileSubN BIGINT UNSIGNED NOT NULL,
    FileSubSize BIGINT UNSIGNED NOT NULL,
    ChildN BIGINT UNSIGNED NOT NULL,
    DescN BIGINT UNSIGNED NOT NULL,
    IsParallel BOOL NOT NULL,
    PRIMARY KEY (Org, Project, Id),
    UNIQUE INDEX(Org, Project, Parent, Id),
    UNIQUE INDEX(Org, Project, NextSib, Id),
    UNIQUE INDEX(Org, Project, Member, Id)
);

DROP TABLE IF EXISTS Vitems;
CREATE TABLE Vitems(
    Org VARCHAR(22) NOT NULL,
    Project VARCHAR(22) NOT NULL,
    Task VARCHAR(22) NOT NULL,
    Type VARCHAR(50) NOT NULL, # time, cost
    Id VARCHAR(22) NOT NULL,
    CreatedBy VARCHAR(22) NOT NULL,
    CreatedOn DATETIME(3) NOT NULL,
    Inc BIGINT UNSIGNED NOT NULL,
    Note VARCHAR(250) NOT NULL,
    PRIMARY KEY(Org, Project, Task, Type, CreatedOn, CreatedBy),
    UNIQUE INDEX(Org, Project, Type, Id),
    UNIQUE INDEX(Org, Project, Task, type, id),
    UNIQUE INDEX(Org, Project, CreatedBy, Type, CreatedOn, Task),
    UNIQUE INDEX(Org, Project, Type, CreatedOn, CreatedBy, Task)
);

DROP TABLE IF EXISTS Files;
CREATE TABLE Files(
    Org VARCHAR(22) NOT NULL,
    Project VARCHAR(22) NOT NULL,
    Task VARCHAR(22) NOT NULL,
    Id VARCHAR(22) NOT NULL,
    Name VARCHAR(250) NOT NULL,
    CreatedBy VARCHAR(22) NOT NULL,
    CreatedOn DATETIME(3) NOT NULL,
    Size BIGINT UNSIGNED NOT NULL,
    Type VARCHAR(250) NOT NULL,
    PRIMARY KEY(Org, Project, Task, CreatedOn, CreatedBy),
    UNIQUE INDEX(Org, Project, Task, Id),
    UNIQUE INDEX(Org, Project, CreatedBy, CreatedOn, Task),
    UNIQUE INDEX(Org, Project, CreatedOn, CreatedBy, Task),
    UNIQUE INDEX(Org, Project, Name, CreatedOn, CreatedBy, Task)
);

DROP TABLE IF EXISTS Comments;
CREATE TABLE Comments(
    Org VARCHAR(22) NOT NULL,
    Project VARCHAR(22) NOT NULL,
    Task VARCHAR(22) NOT NULL,
    Id VARCHAR(22) NOT NULL,
    CreatedBy VARCHAR(22) NOT NULL,
    CreatedOn DATETIME(3) NOT NULL,
    Body VARCHAR(10000) NOT NULL,
    PRIMARY KEY(Org, Project, Task, CreatedOn, CreatedBy),
    UNIQUE INDEX(Org, Project, Task, Id),
    UNIQUE INDEX(Org, Project, CreatedBy, CreatedOn, Task),
    UNIQUE INDEX(Org, Project, CreatedOn, CreatedBy, Task)
);

#!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!#
#********************************MAGIC PROCEDURE WARNING*********************************#
# THIS PROCEDURE MUST ONLY BE CALLED INTERNALLY BY 
# SetAncestralChainAggregateValuesFromTask
#!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!#
DROP PROCEDURE IF EXISTS SetAncestralChainAggregateValuesFromTask;
DELIMITER //
CREATE PROCEDURE SetAncestralChainAggregateValuesFromTask(_org VARCHAR(22), _project VARCHAR(22), _task VARCHAR(22))
BEGIN
  
  DECLARE curParent VARCHAR(22) DEFAULT NULL;

  DECLARE curTimeSubMin BIGINT UNSIGNED DEFAULT 0;
  DECLARE curTimeSubEst BIGINT UNSIGNED DEFAULT 0;
  DECLARE curTimeSubInc BIGINT UNSIGNED DEFAULT 0;
  DECLARE curCostSubEst BIGINT UNSIGNED DEFAULT 0;
  DECLARE curCostSubInc BIGINT UNSIGNED DEFAULT 0;
  DECLARE curFileSubN BIGINT UNSIGNED DEFAULT 0;
  DECLARE curFileSubSize BIGINT UNSIGNED DEFAULT 0;
  DECLARE curChildN BIGINT UNSIGNED DEFAULT 0;
  DECLARE curDescN BIGINT UNSIGNED DEFAULT 0;

  DECLARE newTimeSubMin BIGINT UNSIGNED DEFAULT 0;
  DECLARE newTimeSubEst BIGINT UNSIGNED DEFAULT 0;
  DECLARE newTimeSubInc BIGINT UNSIGNED DEFAULT 0;
  DECLARE newCostSubEst BIGINT UNSIGNED DEFAULT 0;
  DECLARE newCostSubInc BIGINT UNSIGNED DEFAULT 0;
  DECLARE newFileSubN BIGINT UNSIGNED DEFAULT 0;
  DECLARE newFileSubSize BIGINT UNSIGNED DEFAULT 0;
  DECLARE newChildN BIGINT UNSIGNED DEFAULT 0;
  DECLARE newDescN BIGINT UNSIGNED DEFAULT 0;

  DROP TEMPORARY TABLE IF EXISTS tempUpdatedIds;
  CREATE TEMPORARY TABLE tempUpdatedIds(
    id VARCHAR(22) NOT NULL,
    PRIMARY KEY (id)
  );

  WHILE _task IS NOT NULL DO

SELECT
    t.Parent,
    t.TimeSubMin,
    t.TimeSubEst,
    t.TimeSubInc,
    t.CostSubEst,
    t.CostSubInc,
    t.FileSubN,
    t.FileSubSize,
    t.ChildN,
    t.DescN,
    CASE t.IsParallel
        WHEN 0 THEN COALESCE(SUM(c.TimeEst + c.TimeSubMin), 0)
        WHEN 1 THEN COALESCE(MAX(c.TimeEst + c.TimeSubMin), 0)
        END,
    COALESCE(SUM(c.TimeEst + c.TimeSubEst), 0),
    COALESCE(SUM(c.TimeInc + c.TimeSubInc), 0),
    COALESCE(SUM(c.CostEst + c.CostSubEst), 0),
    COALESCE(SUM(c.CostInc + c.CostSubInc), 0),
    COALESCE(SUM(c.FileN + c.FileSubN), 0),
    COALESCE(SUM(c.FileSize + c.FileSubSize), 0),
    COALESCE(COUNT(DISTINCT c.Id), 0),
    COALESCE(COALESCE(COUNT(DISTINCT c.Id), 0) + COALESCE(SUM(c.DescN), 0), 0)
INTO
    curParent,
    curTimeSubMin,
    curTimeSubEst,
    curTimeSubInc,
    curCostSubEst,
    curCostSubInc,
    curFileSubN,
    curFileSubSize,
    curChildN,
    curDescN,
    newtimeSubMin,
    newtimeSubEst,
    newTimeSubInc,
    newCostSubEst,
    newCostSubInc,
    newFileSubN,
    newFileSubSize,
    newChildN,
    newDescN
FROM
    tasks t
        LEFT JOIN
    tasks c
    ON
                c.Org=_org
            AND
                c.Project=_project
            AND
                c.Parent=_task
WHERE
        t.Org=_org
  AND
        t.Project=_project
  AND
        t.Id=_task
GROUP BY
    t.Id;

IF curTimeSubMin <> newtimeSubMin OR
      curTimeSubEst <> newtimeSubEst OR
      curTimeSubInc <> newTimeSubInc OR
      curCostSubEst <> newCostSubEst OR
      curCostSubInc <> newCostSubInc OR
      curFileSubN <> newFileSubN OR
      curFileSubSize <> newFileSubSize OR
      curChildN <> newChildN OR
      curDescN <> newDescN THEN

UPDATE
    tasks
SET
    TimeSubMin=newtimeSubMin,
    TimeSubEst=newtimeSubEst,
    TimeSubInc=newTimeSubInc,
    CostSubEst=newCostSubEst,
    CostSubInc=newCostSubInc,
    FileSubN=newFileSubN,
    FileSubSize=newFileSubSize,
    ChildN=newChildN,
    DescN=newDescN
WHERE
        Org=_org
  AND
        Project=_project
  AND
        Id=_task;

INSERT INTO tempUpdatedIds VALUES (_task);

SET _task = curParent;

ELSE

      SET _task = NULL;

END IF;

END WHILE;

SELECT id FROM tempUpdatedIds;
END //
DELIMITER ;

#useful helper query for manual verifying/debugging test results
#SELECT  t1.name, t2.name AS parent, t3.name AS nextSib, t4.name AS firstChild, t1.description, t1.timeEst, t1.timeInc, t1.timeSubMin, t1.timeSubEst, t1.timeSubInc, t1.costEst, t1.costInc, t1.costSubEst, t1.costSubInc, t1.childN, t1.descN FROM trees_data.tasks t1 LEFT JOIN trees_data.tasks t2 ON t1.parent = t2.id LEFT JOIN trees_data.tasks t3 ON t1.nextSib = t3.id LEFT JOIN trees_data.tasks t4 ON t1.firstChild = t4.id ORDER BY t1.name;


DROP USER IF EXISTS 'Oak'@'%';
CREATE USER 'Oak'@'%' IDENTIFIED BY 'C0-Mm-0n-Oak';
GRANT SELECT ON Oak.* TO 'Oak'@'%';
GRANT INSERT ON Oak.* TO 'Oak'@'%';
GRANT UPDATE ON Oak.* TO 'Oak'@'%';
GRANT DELETE ON Oak.* TO 'Oak'@'%';
GRANT EXECUTE ON Oak.* TO 'Oak'@'%';