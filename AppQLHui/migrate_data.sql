USE [xang4739_HuiDB];
GO

PRINT 'Migrating Users...';
SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] (Id,Username,PasswordHash,FullName,Role,CreatedAt,IsEnabled,BankAccountName,BankAccountNumber,BankName) VALUES (1,N'admin',N'$2a$11$xBIhrBEx0AvfgzHJGvK0S.OfRCbHNz2Da4lSas2iFCGhZmq4yexCa',N'Administrator',0,N'2026-04-03 17:49:06.8096091',1,NULL,NULL,NULL);
INSERT INTO [Users] (Id,Username,PasswordHash,FullName,Role,CreatedAt,IsEnabled,BankAccountName,BankAccountNumber,BankName) VALUES (2,N'test_owner_2',N'$2a$11$3T6KxkTSssgzcC485PFQLeN4eYUQnzfBbwbOPffSCftI2XFUuAcnu',N'Ch',1,N'2026-04-03 18:36:31.5927755',1,NULL,NULL,NULL);
SET IDENTITY_INSERT [Users] OFF;
GO

PRINT 'Migrating Players...';
SET IDENTITY_INSERT [Players] ON;
INSERT INTO [Players] (Id,Name,Phone,ZaloName,CreatedAt,BankAccountName,BankAccountNumber,BankName,OwnerId,Notes) VALUES (6,N'Nguy?n Van Minh',N'0901234567',NULL,N'2026-04-03 12:30:16.8661958',NULL,NULL,NULL,1,NULL);
INSERT INTO [Players] (Id,Name,Phone,ZaloName,CreatedAt,BankAccountName,BankAccountNumber,BankName,OwnerId,Notes) VALUES (7,N'Tr?n Th? An',N'0902345678',NULL,N'2026-04-03 12:30:20.7765292',NULL,NULL,NULL,1,NULL);
INSERT INTO [Players] (Id,Name,Phone,ZaloName,CreatedAt,BankAccountName,BankAccountNumber,BankName,OwnerId,Notes) VALUES (8,N'Lê Van Hùng',N'0903456789',NULL,N'2026-04-03 12:30:24.0871565',NULL,NULL,NULL,1,NULL);
SET IDENTITY_INSERT [Players] OFF;
GO

PRINT 'Migrating Tontines...';
SET IDENTITY_INSERT [Tontines] ON;
INSERT INTO [Tontines] (Id,Name,Type,BaseAmount,TotalShares,CommissionFee,FeeType,Status,CreatedAt,TotalFeesCollected,CycleType,OwnerId,CycleNotes) VALUES (1,N'Dây H?i Tình Nghia 1',0,1000000,3,50000,1,1,N'2026-04-03 12:30:40.7283820',100000,N'',1,NULL);
INSERT INTO [Tontines] (Id,Name,Type,BaseAmount,TotalShares,CommissionFee,FeeType,Status,CreatedAt,TotalFeesCollected,CycleType,OwnerId,CycleNotes) VALUES (2,N'Ngày 1 tri?u 20 ph?n',0,1000000,20,150000,1,1,N'2026-04-03 12:36:44.1334541',150000,N'',1,NULL);
INSERT INTO [Tontines] (Id,Name,Type,BaseAmount,TotalShares,CommissionFee,FeeType,Status,CreatedAt,TotalFeesCollected,CycleType,OwnerId,CycleNotes) VALUES (3,N'Ngày 100k 15 ph?n',0,100000,15,15000,1,0,N'2026-04-03 13:55:53.5088298',0,N'',1,NULL);
INSERT INTO [Tontines] (Id,Name,Type,BaseAmount,TotalShares,CommissionFee,FeeType,Status,CreatedAt,TotalFeesCollected,CycleType,OwnerId,CycleNotes) VALUES (4,N'Day Test Search',0,500000,3,100000,1,1,N'2026-04-03 14:09:34.1565596',0,N'',1,NULL);
INSERT INTO [Tontines] (Id,Name,Type,BaseAmount,TotalShares,CommissionFee,FeeType,Status,CreatedAt,TotalFeesCollected,CycleType,OwnerId,CycleNotes) VALUES (5,N'Test Tontine',0,1000000,10,50000,1,0,N'2026-04-03 14:33:59.6179085',0,N'Ngày',1,NULL);
INSERT INTO [Tontines] (Id,Name,Type,BaseAmount,TotalShares,CommissionFee,FeeType,Status,CreatedAt,TotalFeesCollected,CycleType,OwnerId,CycleNotes) VALUES (6,N'Test Tontine',0,1000000,10,50000,1,0,N'2026-04-03 14:34:43.4129849',0,N'Ngày',1,NULL);
INSERT INTO [Tontines] (Id,Name,Type,BaseAmount,TotalShares,CommissionFee,FeeType,Status,CreatedAt,TotalFeesCollected,CycleType,OwnerId,CycleNotes) VALUES (7,N'Ngày 1 tri?u 50 ph?n',0,1000000,50,1500000,1,1,N'2026-04-03 14:39:14.2688654',0,N'Ngày khui 2 l?n',1,NULL);
SET IDENTITY_INSERT [Tontines] OFF;
GO

PRINT 'Migrating TontineShares...';
SET IDENTITY_INSERT [TontineShares] ON;
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (1,1,1,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (2,1,2,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (3,1,3,8,1,3,1);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (4,2,1,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (5,2,2,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (6,2,3,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (7,2,4,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (8,2,5,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (9,2,6,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (10,2,7,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (11,2,8,6,1,2,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (12,2,9,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (13,2,10,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (14,2,11,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (15,2,12,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (16,2,13,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (17,2,14,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (18,2,15,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (19,2,16,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (20,2,17,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (21,2,18,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (22,2,19,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (23,2,20,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (24,4,1,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (25,4,2,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (26,4,3,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (27,7,1,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (28,7,2,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (29,7,3,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (30,7,4,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (31,7,5,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (32,7,6,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (33,7,7,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (34,7,8,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (35,7,9,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (36,7,10,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (37,7,11,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (38,7,12,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (39,7,13,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (40,7,14,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (41,7,15,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (42,7,16,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (43,7,17,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (44,7,18,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (45,7,19,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (46,7,20,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (47,7,21,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (48,7,22,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (49,7,23,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (50,7,24,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (51,7,25,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (52,7,26,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (53,7,27,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (54,7,28,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (55,7,29,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (56,7,30,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (57,7,31,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (58,7,32,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (59,7,33,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (60,7,34,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (61,7,35,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (62,7,36,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (63,7,37,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (64,7,38,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (65,7,39,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (66,7,40,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (67,7,41,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (68,7,42,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (69,7,43,6,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (70,7,44,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (71,7,45,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (72,7,46,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (73,7,47,8,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (74,7,48,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (75,7,49,7,0,NULL,0);
INSERT INTO [TontineShares] (Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly) VALUES (76,7,50,7,0,NULL,0);
SET IDENTITY_INSERT [TontineShares] OFF;
GO

PRINT 'Migrating Draws...';
SET IDENTITY_INSERT [Draws] ON;
INSERT INTO [Draws] (Id,TontineId,SequenceNumber,DrawDate,WinningShareId,BidAmount,CollectedFee,ActualReceived,OldDebtDeduction) VALUES (2,2,1,N'2026-04-03 09:35:49.5000000',11,200000,150000,0,0);
INSERT INTO [Draws] (Id,TontineId,SequenceNumber,DrawDate,WinningShareId,BidAmount,CollectedFee,ActualReceived,OldDebtDeduction) VALUES (3,1,1,N'2026-04-03 10:12:56.2980000',3,100000,50000,0,0);
SET IDENTITY_INSERT [Draws] OFF;
GO

PRINT 'Migrating Transactions...';
SET IDENTITY_INSERT [Transactions] ON;
INSERT INTO [Transactions] (Id,DrawId,PlayerId,AmountPay,AmountReceive,NetAmount,IsSettled,PaidAmount) VALUES (4,2,6,3200000,15050000,11850000,0,0);
INSERT INTO [Transactions] (Id,DrawId,PlayerId,AmountPay,AmountReceive,NetAmount,IsSettled,PaidAmount) VALUES (5,2,8,2400000,0,-2400000,0,0);
INSERT INTO [Transactions] (Id,DrawId,PlayerId,AmountPay,AmountReceive,NetAmount,IsSettled,PaidAmount) VALUES (6,2,7,9600000,0,-9600000,0,0);
INSERT INTO [Transactions] (Id,DrawId,PlayerId,AmountPay,AmountReceive,NetAmount,IsSettled,PaidAmount) VALUES (7,3,8,0,1750000,1750000,0,0);
INSERT INTO [Transactions] (Id,DrawId,PlayerId,AmountPay,AmountReceive,NetAmount,IsSettled,PaidAmount) VALUES (8,3,6,1800000,0,-1800000,0,0);
SET IDENTITY_INSERT [Transactions] OFF;
GO

PRINT 'Migrating ZaloSettings...';
SET IDENTITY_INSERT [ZaloSettings] ON;
INSERT INTO [ZaloSettings] (Id,SendDelayMs,PasteX,PasteY,PasteShortcut,SendShortcut,IsSingleSendOnly,AutoCopyBill,LastUpdated,OwnerId) VALUES (1,1000,0,0,N'Ctrl+V',N'Enter',1,1,N'2026-04-03 13:56:26.7134960',1);
SET IDENTITY_INSERT [ZaloSettings] OFF;
GO

