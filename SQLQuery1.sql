CREATE procedure sp_InsertProducts
(
@ProductName nvarchar(50),
@Price decimal(8,2),
@Qty int,
@Remarks nvarchar(50)=null
)
as
begin
        begin try
insert into tbl_ProductMaster(ProductName,Price,Qty,Remarks)
values(@ProductName,@Price,@Qty,@Remarks)
commit tran
end try
begin catch
rollback tran
select Error_Message()
end catch
end