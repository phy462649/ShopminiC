-- Migration: Add google_id column to person table for Google OAuth login
-- Run this SQL on your MySQL database

ALTER TABLE `person` 
ADD COLUMN `google_id` VARCHAR(255) NULL DEFAULT NULL 
AFTER `otp`;

-- Optional: Add index for faster lookup by google_id
CREATE INDEX `idx_person_google_id` ON `person` (`google_id`);
