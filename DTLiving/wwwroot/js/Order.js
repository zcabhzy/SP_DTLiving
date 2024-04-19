document.addEventListener("DOMContentLoaded", function () {
    GetOrderDetail();
});

function GetOrderDetail() {

    var token = localStorage.getItem('JwtToken');

    fetch('/OrderTable/ShowOrder', {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + token
        }
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('無法獲取訂單');
            }
        })
        .then(data => {

            console.log(JSON.stringify(data))

            var orderTableItem = document.getElementById('OrderTable');
            orderTableItem.innerHTML = '';

            data.forEach(order => {
                // 創建訂單卡片元素
                let orderBox = document.createElement('div');
                orderBox.className = 'card mb-3';

                // 訂單編號和訂單時間的容器
                let orderInfoContainer = document.createElement('div');
                orderInfoContainer.className = 'card-header d-flex ';
                orderInfoContainer.style.justifyContent = 'center';

                // 訂單編號
                let orderNumberCol = document.createElement('div');
                orderNumberCol.className = 'col';
                let orderNumber = document.createElement('h5');
                orderNumber.textContent = '訂單編號: ' + order.orderNumber;
                orderNumber.className = 'm-0';
                orderNumberCol.appendChild(orderNumber);

                // 訂單時間
                let orderTimeCol = document.createElement('div');
                orderTimeCol.className = 'col text-right';
                let orderTime = document.createElement('h5');
                orderTime.textContent = order.createTime;
                orderTime.className = 'm-0';
                orderTimeCol.appendChild(orderTime);

                orderInfoContainer.appendChild(orderNumberCol);
                orderInfoContainer.appendChild(orderTimeCol);
                orderBox.appendChild(orderInfoContainer);

                // 商品名稱/數量/小計的容器
                let orderInfoContainer2 = document.createElement('div');
                orderInfoContainer2.className = 'card-body row ';

                let productNameCol = document.createElement('div');
                productNameCol.className = 'col-8';
                let productName = document.createElement('h6');
                productName.textContent = '商品名稱';
                productNameCol.appendChild(productName);

                let productCountCol = document.createElement('div');
                productCountCol.className = 'col-2 text-center';
                let productCount = document.createElement('h6');
                productCount.textContent = '數量';
                productCountCol.appendChild(productCount);

                let productSubtotalCol = document.createElement('div');
                productSubtotalCol.className = 'col-2 text-center';
                let productSubtotal = document.createElement('h6');
                productSubtotal.textContent = '小計';
                productSubtotalCol.appendChild(productSubtotal);

                orderInfoContainer2.appendChild(productNameCol);
                orderInfoContainer2.appendChild(productCountCol);
                orderInfoContainer2.appendChild(productSubtotalCol);

                orderBox.appendChild(orderInfoContainer2);

                // 填充訂單商品項目
                order.items.forEach(item => {
                    let orderItemContainer = document.createElement('div');
                    orderItemContainer.className = 'card-body row';

                    let itemNameCol = document.createElement('div');
                    itemNameCol.className = 'col-8';
                    let itemName = document.createElement('p');
                    itemName.textContent = item.name;
                    itemNameCol.appendChild(itemName);

                    let itemQuantityCol = document.createElement('div');
                    itemQuantityCol.className = 'col-2 text-center';
                    let itemQuantity = document.createElement('p');
                    itemQuantity.textContent = item.quantity;
                    itemQuantityCol.appendChild(itemQuantity);

                    let itemPriceCol = document.createElement('div');
                    itemPriceCol.className = 'col-2 text-center';
                    let itemPrice = document.createElement('p');
                    itemPrice.textContent = item.price;
                    itemPriceCol.appendChild(itemPrice);

                    orderItemContainer.appendChild(itemNameCol);
                    orderItemContainer.appendChild(itemQuantityCol);
                    orderItemContainer.appendChild(itemPriceCol);

                    orderBox.appendChild(orderItemContainer);
                });

                // 收件人資訊
                let recipientInfo = document.createElement('div');
                recipientInfo.className = 'card-footer';

                let totalPrice = document.createElement('p');
                totalPrice.textContent = '總計 : ' + order.total;
                recipientInfo.appendChild(totalPrice);

                let recipient = document.createElement('p');
                recipient.textContent = '收件人 : ' + order.receiver;
                recipientInfo.appendChild(recipient);

                let phone = document.createElement('p');
                phone.textContent = '收件人電話 : ' + order.clientInfo.userPhone;
                recipientInfo.appendChild(phone);

                let address = document.createElement('p');
                address.textContent = '收件人地址 : ' + order.clientInfo.userAddress;
                recipientInfo.appendChild(address);

                let paymentStatus = document.createElement('p');
                paymentStatus.textContent = '付款狀態: ' + (order.isPad ? '已付款' : '未付款');
                recipientInfo.appendChild(paymentStatus);

                orderBox.appendChild(recipientInfo);

                // PayPal 付款按鈕
                let paypalButtonContainer = document.createElement('button');
                paypalButtonContainer.id = 'payment-button-container';
                paypalButtonContainer.className = 'card-footer btn btn-dark';
                paypalButtonContainer.style.border = '1px solid gray'
                paypalButtonContainer.textContent = '付款';
                paypalButtonContainer.addEventListener('click', function () {

                    IsPadCheckout(order, data);
                    toastr['success']('付款成功');
                    setTimeout(function () {
                        window.location.href = "Order.html";
                    }, 5000);

                });
                orderBox.appendChild(paypalButtonContainer);

                orderTableItem.appendChild(orderBox);
            });
        })
        .catch(error => {
            console.log('error')
        })
}

function IsPadCheckout(order, data) {
    var token = localStorage.getItem('JwtToken');

    var requestBody = order.orderNumber;

    fetch('/OrderTable/IsPadCheck', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestBody)

    }).then(response => {
        if (response.ok) {
            // 若請求成功，處理後端返回的訊息
            return response.text().then(message => {

                console.log("後端回應：" + message); // 確保 message 正確包含订单编号
            });
        } else {
            // 若請求失敗，拋出錯誤
            throw new Error('請求失敗，HTTP 狀態碼：' + response.status);
        }
    }).catch(error => {
        console.error("發生錯誤：" + error);
    });
}

// #endregion