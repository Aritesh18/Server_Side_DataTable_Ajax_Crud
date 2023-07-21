alter PROCEDURE sp_GetSelectedEmployees
    @SelectedEmployeeIds dbo.EmployeeIdList READONLY 
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM Employees
    WHERE EmployeeId IN (SELECT EmployeeId FROM @SelectedEmployeeIds)
END


CREATE TYPE dbo.EmployeeIdList AS TABLE
(
    EmployeeId INT
)
