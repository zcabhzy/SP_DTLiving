// #region Function => fetchProductData() 抓取所有商品
async function fetchProductData() {

  const apiUrl = "/ProductCache/GetProduct";
  const response = await fetch(apiUrl);

  if (!response.ok) {
    throw new Error(`HTTP error! Status: ${response.status}`);
  }

  const data = await response.json();
  handleJsonData(data);

  console.log(data)
}
// #endregion

// #region Function => handleJsonData() 動態生成商品
function handleJsonData(data) {
  const productContainer = document.getElementById("ProductContainer");
  productContainer.innerHTML = "";
  data.forEach((product) => {
    setProduct(productContainer, product);
  });
}
// #endregion

// #region Function => setProduct() 渲染商品 / 加入購物車
function setProduct(container, product) {

  const productInfo = document.createElement("div");
  productInfo.style.maxWidth = "300px";
  productInfo.style.maxHeight = "300px";
  productInfo.style.display = "flex";
  productInfo.style.flexDirection = "column";

  const productImage = document.createElement("img");
  productImage.className = "PImage";
  productImage.src = `data:image/jpeg;base64,${product.image}`;
  productImage.style.width = "195px";
  productImage.style.height = "130px";
  productImage.style.margin = "auto";

  const productName = document.createElement("a");
  productName.className = "PName";
  productName.style.color = 'black';
  productName.innerText = product.name;
  productName.style.textAlign = "left";
  productName.style.paddingLeft = "5px";
  productName.style.fontWeight = 600 ;
  productName.style.textDecoration = 'none';


  const productCategory = document.createElement("span");
  productCategory.classList = "PCategory";
  productCategory.style.textAlign = "left";
  productCategory.style.paddingLeft = "5px";
  productCategory.innerText = product.category;

  productInfo.appendChild(productImage);
  productInfo.appendChild(productName);
  productInfo.appendChild(productCategory);
  container.appendChild(productInfo);

  const productid = product.id;

  productName.addEventListener('click', async function () {

    const productContainer = document.getElementById("ProductContainer");
    productContainer.innerHTML = '';

    /** productdata 用以儲存 API : getProductDataId() 抓取的資料 */
    const productdata = await getProductDataId(productid);

    const detailContainer = document.getElementById("DetailContainer");
    detailContainer.style.display = 'block';

    const detailImage = detailContainer.querySelector('#detailimage');
    detailImage.src = `data:image/jpeg;base64,${productdata.image}`;

    const detailName = detailContainer.querySelector('#detailname');
    detailName.innerText = productdata.name;

    const detailContext = detailContainer.querySelector('#detailcontext');
    detailContext.innerText = productdata.context;

    const detailCategory = detailContainer.querySelector('#detailcategory');
    detailCategory.innerText = `設計品牌 ${productdata.category}`;

    const detailPrice = detailContainer.querySelector('#detailprice');
    detailPrice.innerText = `商品價格 ${productdata.price}`;

    const detailStock = detailContainer.querySelector('#detailstock');
    detailStock.innerText = `商品數量 ${productdata.stock}`;

    // 將使用者購買數量加入 JSON 資料內
    const detailQuantityInput = document.getElementById('quantity');

    const CarBtn = document.getElementById('AddProductBtn');

    // #region Function => CarBtn.addEventListener() 加入購物車
    CarBtn.addEventListener('click', function () {

      var token = localStorage.getItem('JwtToken');

      if (token) {
        const enteredQuantity = parseInt(detailQuantityInput.value, 10);

        if (enteredQuantity > productdata.stock) {

          alert("購買數量已達到上限");

        } else {

          const ProductJSON = {

            id: productdata.id,
            name: productdata.name,
            image: productdata.image,
            category: productdata.category,
            price: productdata.price,
            stock: productdata.stock,
            quantity: parseInt(detailQuantityInput.value, 10)

          };

          AddProducts(ProductJSON);

        }

      } else {

        alert("請先登入會員歐")

      }

    });
    // #endregion

  });
}
// #endregion

fetchProductData();
