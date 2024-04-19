// #region Function => AddProducts()      加入購物車
function AddProducts(productdata) {

    let cartData = JSON.parse(localStorage.getItem('cart')) || [];

    const existingProduct = cartData.find(item => item.id === productdata.id);

    if (existingProduct) {

        toastr['success']('商品已存在於購物車中!');

    } else {

        toastr['success']('成功加入購物車');
        
        cartData.push(productdata);

        localStorage.setItem('cart', JSON.stringify(cartData));
    }
}
// #endregion

// #region Function => getProductDataId() 取得特定產品
async function getProductDataId(productid) {

    const apiUrl = `/ProductCache/GetProductDetail?productid=${productid}`;
    const response = await fetch(apiUrl);

    let DetailData;

    if (!response.ok) {

        throw new Error(`HTTP error! Status: ${response.status}`);

    } else {

        DetailData = await response.json();
    }

    return DetailData;
}
// #endregion

// #region Function => increaseQuantity() 增加商品數量，檢查是否超過在庫數
function increaseQuantity() {

    const warningSpan = document.querySelector('#warningspan');
    var quantityInput = document.getElementById('quantity');
    var currentQuantity = parseInt(quantityInput.value, 10);
    var productStock = parseInt(document.getElementById('detailstock').innerText.split(' ')[1], 10);

    if (currentQuantity < productStock) {
        currentQuantity++;
        quantityInput.value = currentQuantity;
    } else {

        toastr['success']('已超過購買數量!');
    }

}
// #endregion

// #region Function => decreaseQuantity() 減少商品數量
function decreaseQuantity() {

    var quantityInput = document.getElementById('quantity');
    var currentQuantity = parseInt(quantityInput.value, 10);

    if (currentQuantity > 1) {

        currentQuantity--;

        quantityInput.value = currentQuantity;
    }
}
// #endregion
