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
    NewEmail VARCHAR(250) NOT NULL,
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

DROP USER IF EXISTS 'Oak'@'%';
CREATE USER 'Oak'@'%' IDENTIFIED BY 'C0-Mm-0n-Oak';
GRANT SELECT ON Oak.* TO 'Oak'@'%';
GRANT INSERT ON Oak.* TO 'Oak'@'%';
GRANT UPDATE ON Oak.* TO 'Oak'@'%';
GRANT DELETE ON Oak.* TO 'Oak'@'%';
GRANT EXECUTE ON Oak.* TO 'Oak'@'%';