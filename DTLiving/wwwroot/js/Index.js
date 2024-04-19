function ChangeImage() {

    const RadioButtons1 = document.getElementById('radioButtons1');
    const RadioButtons2 = document.getElementById('radioButtons2');

    let Imgs1 = document.getElementById('showimage1');
    let Imgs2 = document.getElementById('showimage2');
    let Imgs3 = document.getElementById('showimage3');
    let Imgs4 = document.getElementById('showimage4');
    let Imgs5 = document.getElementById('showimage5');

    RadioButtons2.addEventListener('change', function () {

        RadioButtons1.checked = false;

        Imgs1.src = 'SImage/Oluce.jpg';
        Imgs2.src = 'SImage/Papadatos.jpg';
        Imgs3.src = 'SImage/PIURE.jpg';
        Imgs4.src = 'SImage/SABA.jpg';
        Imgs5.src = 'SImage/Safretti.jpg';

    });

    RadioButtons1.addEventListener('change', function () {

        RadioButtons2.checked = false;

        Imgs1.src = 'SImage/Arketipo.jpg';
        Imgs2.src = 'SImage/Artisan.jpg';
        Imgs3.src = 'SImage/FRAG.jpg';
        Imgs4.src = 'SImage/La Chance.jpg';
        Imgs5.src = 'SImage/Oluce.jpg';

    });

}