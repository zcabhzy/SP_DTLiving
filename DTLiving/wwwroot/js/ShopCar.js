// #region Function => ConfirmOrder() 建立訂單
function ConfirmOrder() {

    const GetLocalData = JSON.parse(localStorage.getItem('cart'));
    const extractedData = [];
    var token = localStorage.getItem('JwtToken');

    if (token == null) {
        toastr['warning']('尚未登入會員歐!!');
        return;
    }

    GetLocalData.forEach(item => {
        const extractedItem = {
            ProductId: item.id,
            Name: item.name,
            OrderCategory: item.category,
            Price: item.price,
            Stock: item.stock,
            Quantity: item.quantity
        };
        extractedData.push(extractedItem);
    });

    fetch('/OrderTable/CreateOrder', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
        },
        body: JSON.stringify(extractedData)
    }).then(response => {
        if (response.ok) {

            localStorage.removeItem('cart');
            console.log("訂單建立成功");
            window.location.href = 'Order.html';

        } else {

            console.error("訂單建立失敗");

        }
    }).catch(error => {

        console.error(error);

    });

}
// #endregion