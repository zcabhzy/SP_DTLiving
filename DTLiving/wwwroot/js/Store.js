const cartData = JSON.parse(localStorage.getItem("cart")) || [];
const cartListElement = document.getElementById("cartList");

cartListElement.innerHTML = "";

// #region Function => renderCart() 整理已加入購物車商品
function renderCart() {

  cartData.forEach((productdata) => {

    const listItem = document.createElement("div");
    listItem.className = "row";
    listItem.style.width = "100%";
    listItem.style.height = "70px";
    listItem.style.display = "flex";
    listItem.style.textAlign = "center";
    listItem.style.justifyContent = "center";
    listItem.style.alignItems = "center";
    listItem.style.backgroundColor = 'rgb(234, 222, 217)';
    listItem.style.border = '1px solid rgb(126, 100, 88)'

    // 創建圖片元素
    const imageElement = document.createElement("img");
    imageElement.className = "col";
    imageElement.src = `data:image/jpeg;base64,${productdata.image}`;
    imageElement.style.height = "50px";
    imageElement.style.width = "80%";
    imageElement.style.maxHeight = "80px";


    // 創建名稱元素
    const nameElement = document.createElement("span");
    nameElement.className = "col";
    nameElement.style.display = "flex";
    nameElement.style.justifyContent = "center";
    nameElement.textContent = `${productdata.name}`;


    // 創建數量元素
    const quantityElement = document.createElement("input");
    const quantity = productdata.quantity || 1;
    quantityElement.className = "col-4";
    quantityElement.style.display = "flex";
    quantityElement.style.textAlign = "center";
    quantityElement.id = 'quantity';
    quantityElement.value = `${productdata.quantity}`;


    const ReduceBtn = document.createElement('button');
    ReduceBtn.className = 'col';
    ReduceBtn.innerHTML = '&#45';
    ReduceBtn.onclick = decreaseQuantity(productdata, quantityElement);


    const AddBtn = document.createElement('button');
    AddBtn.className = 'col';
    AddBtn.innerHTML = '&#43';
    AddBtn.onclick = increaseQuantity(productdata, quantityElement);


    // 創建價格元素
    const priceElement = document.createElement("span");
    priceElement.className = "col";
    priceElement.style.display = "flex";
    priceElement.style.justifyContent = "center";
    priceElement.textContent = `$${productdata.price}`;

    listItem.appendChild(imageElement);
    listItem.appendChild(nameElement);
    listItem.appendChild(ReduceBtn);
    listItem.appendChild(quantityElement);
    listItem.appendChild(AddBtn);
    listItem.appendChild(priceElement);
    cartListElement.appendChild(listItem);

  });

}

// #endregion

// #region Function => updateCart() 購物車總計

function updateCart() {

  localStorage.setItem("cart", JSON.stringify(cartData));

  const totalPrice = getTotalProductQuantity();
  GetProductTotal.innerHTML = `小計：$${totalPrice}`;

  const totalProductQuantity = getTotalProductQuantity();
  GetTotal.innerHTML = `總額：$${totalProductQuantity}`;

}

// #endregion

renderCart();

// #region Function => DeleteShopCar() 清除購物車
function DeleteShopCar() {

  if (localStorage.getItem('cart')) {

    localStorage.removeItem('cart');

    alert('確認刪除購物車商品 ?');

    location.reload();

  } else {

    alert('購物車目前沒有商品歐.');

  }
}
// #endregion

// #region Function => increaseQuantity() 增加購物車商品數量
function increaseQuantity(productdata, quantityElement) {
  return function () {

    const currentQuantity = parseInt(quantityElement.value, 10);

    productdata.quantity++;

    if (productdata.quantity > productdata.stock) {

      toastr['success']('已超過購買上限，請勿重複點擊')

    } else {

      quantityElement.value = `${productdata.quantity}`;
      updateCart();
    }

  }
}

// #endregion

// #region Function =>  decreaseQuantity() 減少購物車商品數量
function decreaseQuantity(productdata, quantityElement) {
  return function () {
    if (productdata.quantity > 1) {
      productdata.quantity--;
      quantityElement.value = `${productdata.quantity}`;
      updateCart();
    }
  }
}
// #endregion

// #region Function => 購物車總計
function getTotalProductQuantity() {

  let TotalPrice = 0;

  cartData.forEach((product) => {
    TotalPrice += (product.price * product.quantity);
  });

  return TotalPrice;
}
// #endregion

// 小計 : 產品價格
let GetProductTotal = document.getElementById('ProductTotal');
const totalPrice = getTotalProductQuantity()
GetProductTotal.innerHTML += totalPrice;

// 總額 : 包含運費後的價格
let GetTotal = document.getElementById('Total');
const totalProductQuantity = getTotalProductQuantity();
GetTotal.innerHTML += totalProductQuantity;

