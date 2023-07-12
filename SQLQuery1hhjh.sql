CREATE TABLE [dbo].[Employee] (
    [EmployeeID] INT           IDENTITY (1, 1) NOT NULL,
    [Name]       VARCHAR (50)  NULL,
    [Age]        INT           NULL,
    [State]      NVARCHAR (55) NULL,
    [Country]    NVARCHAR (55) NULL,
    CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([EmployeeID] ASC)
);
drop table Employee
