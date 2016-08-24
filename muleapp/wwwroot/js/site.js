function getRandomInt(a, b) { return Math.floor(Math.random() * (b - a + 1)) + a; }
function getRandomPony() { return getRandomInt(0, 2) * getRandomInt(0, 3) * getRandomInt(0, 4) + 1; }
var altura = 800, largura = 4000, rainbow = null;
function egg1() {
    if ($(".nyan").length !== 1) { return; } //Credit: https://codepen.io/brunorcunha/
    largura = parseInt($("body").width()); function criarEstrela() { var a = getRandomInt(3, 14), b = getRandomInt(10, 40), c = $('<a class="star" href="/error/Sudden Death"></>').css({ width: a + "px", height: a + "px", left: largura - 10 + "px", top: Math.floor(Math.random() * altura + 1), "-webkit-transition": "all " + b + "s linear", "-webkit-transform": "translate(0px, 0px)" }); $("body").append(c), window.setTimeout(function () { c.css({ "-webkit-transform": "translate(-" + largura + "px, 0px)" }) }, 10 * getRandomInt(5, 10)), window.setTimeout(function () { c.remove() }, 1e3 * b) } window.setInterval(function () { criarEstrela() }, 300);
};
function egg2() {
    if ($(".nyan").length !== 1 || rainbow !== null) { return; } //Credit: https://codepen.io/brunorcunha/
    var tamanhoTela = parseInt(largura / 9); function moveNyan() { var a = nyan.width() / 2, b = nyan.height() / 2; px += (posX - px - a) / 50, py += (posY - py - b) / 50, nyan.css({ left: px + "px", top: py + "px" }) } function peidaArcoIris() { var a = Math.floor(nyan.position().left / 9) + 2; pilha.length >= a && pilha.pop(), pilha.unshift(py), rainbow.hide(); for (var b = 0; b < a; b++) { var c = b % 2; an && (c = b % 2 ? 0 : 1), rainbow.eq(a - b).css({ top: pilha[b] + c }).show() } } var posX = 100, posY = 100, px = 0, py = 0, an = !1, nyan = $(".nyan"), pilha = []; $(document).on("mousemove", function (a) { posX = a.pageX, posY = a.pageY }); for (var i = 0; i < tamanhoTela; i++) { var rain = $('<div class="rainbow"/>').css("left", 9 * i + "px"); $("body").append(rain) } rainbow = $(".rainbow"), window.setInterval(function () { moveNyan(), peidaArcoIris() }, 10), window.setInterval(function () { an = !an }, 500); var frame = 0; window.setInterval(function () { nyan.css({ "background-position": 34 * frame + "px" }), frame++ }, 100);
};
function egg3() {
    $("#unikitty").attr("src", "/img/unikitty2.png");
    $("#unikitty").wrap($('<a>', { href: '/' }));
};