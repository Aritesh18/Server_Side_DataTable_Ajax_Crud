ALTER PROCEDURE sp_GetSelectedEmployees
    @SelectedEmployeeIDs SelectedEmployeeIDsType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    SELECT e.EmployeeID, e.FirstName, e.LastName, e.Age, e.State, e.Country
    FROM Employee e
    INNER JOIN @SelectedEmployeeIDs ids ON e.EmployeeID = ids.EmployeeID;
END