// straight
// var coords = [195, 5, 5, 5, 220, 195, 25, 195, 50, 100, 175, 100];
// angled
// var coords =  [195, 5, 5, 30, 220, 170, 25, 195, 50, 95, 175, 80];
// long style
var coords =  [180, 25, 5, 20, 75, 330, 50, 395, 71, 115, 155, 110];
var hit = -1;
function paint(canvas) {
    var ctx = canvas.getContext("2d");
    //hook_events(canvas);
    ctx.beginPath();
    ctx.strokeStyle = "rgb(0, 0, 128)";
    ctx.beginPath();
    ctx.moveTo(coords[0], coords[1]);
    ctx.bezierCurveTo(coords[2], coords[3], coords[4], coords[5], coords[6], coords[7]);
    ctx.stroke();
    ctx.strokeStyle = "rgba(0, 128, 0, .5)";
    ctx.beginPath();
    ctx.moveTo(coords[0], coords[1]);
    ctx.lineTo(coords[2], coords[3]);
    ctx.stroke();
    ctx.beginPath();
    ctx.moveTo(coords[4], coords[5]);
    ctx.lineTo(coords[6], coords[7]);
    ctx.stroke();
    ctx.beginPath();
    ctx.strokeStyle = "rgb(0, 0, 128)";
    ctx.moveTo(coords[8], coords[9]);
    ctx.lineTo(coords[10], coords[11]);
    ctx.stroke();
    for (var i = 0; i < 11; i += 2) {
        ctx.beginPath();
        ctx.arc(coords[i], coords[i + 1], 2, 0, 2 * Math.PI, false);
        ctx.fill();
    }
}
function repaint(canvas) {
    var ctx = canvas.getContext("2d");
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    paint(canvas);
}
function mousedown(evt, canvas) {
    var x = evt.offsetX != undefined ? evt.offsetX : evt.pageX - canvas.offsetLeft;
    var y = evt.offsetY != undefined ? evt.offsetY : evt.pageY - canvas.offsetTop;
    hit = -1;
    for (var i = 0; i < 6; i++) {
        r2 = (coords[i * 2] - x) * (coords[i * 2] - x) +
             (coords[i*2+1] - y) * (coords[i*2+1] - y);
        if (r2 < 10) {
	    hit = i;
            break;
        }
    }
}
function mousemove(evt, canvas) {
    if (hit >= 0) {
	var x = evt.offsetX != undefined ? evt.offsetX : evt.pageX - canvas.offsetLeft;
	var y = evt.offsetY != undefined ? evt.offsetY : evt.pageY - canvas.offsetTop;
        coords[hit * 2] = x;
        coords[hit * 2 + 1] = y;
	var canvas = document.getElementById("canvas");
        repaint(canvas);
    }
}
function init() {
    var canvas = document.getElementById("canvas");
    canvas.onmousedown = function(evt) { mousedown(evt, canvas); evt.preventDefault(); };
    canvas.onmouseup = function(evt) { hit = -1; evt.preventDefault(); };
    canvas.onmousemove = function(evt) { mousemove(evt, canvas); evt.preventDefault(); };
    paint(canvas);
}
