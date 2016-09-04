function draw_spiro(ctx, k0, k1, k2, k3) {
    if (!this.x) { this.x = .5 }
    this.x += .1;
    ctx.moveTo(-.5, 0);
    ctx.lineTo(.5, 0);
}
function show_xy_evt(evt, style) {
    var canvas = document.getElementById("canvas");
    var ctx = canvas.getContext("2d");
    var x = evt.offsetX != undefined ? evt.offsetX : evt.pageX - canvas.offsetLeft;
    var y = evt.offsetY != undefined ? evt.offsetY : evt.pageY - canvas.offsetTop;
    ctx.beginPath();
    ctx.fillStyle = style;
    ctx.moveTo(x-1, y-1);
    ctx.lineTo(x+1, y-1);
    ctx.lineTo(x+1, y+1);
    ctx.lineTo(x-1, y+1);
    ctx.fill();
}
function show_evt(evt) {
    var str = "[";
    for (x in evt) {
	str += x + ": " 
	    try {
		str += evt[x] + ", ";
	    } catch (e) {
		str += "<nodefault>";
	    }
    }
    str += "]";
    var msgel = document.getElementById("msg");
    msgel.appendChild(document.createTextNode(str));
}
function hook_events(canvas) {
    show_evt(canvas);
    canvas.onmouseover = function(evt) { show_evt(evt, "red"); evt.preventDefault(); };
    canvas.onmouseup = function(evt) { show_evt(evt, "red"); evt.preventDefault(); };
    canvas.onmousedown = function(evt) { show_xy_evt(evt, "red"); evt.preventDefault(); };
    //canvas.onmousemove = function(evt) { show_xy_evt(evt, "black"); evt.preventDefault(); };
    canvas.ondblclick = function() { alert("varclick") };
    window.addEventListener("keypress", function(evt){
	    show_evt(evt);
	    evt.preventDefault();
	}, false);
}

// -- generic band-diagonal matrix solver, adapted from numerical recipes

function bandec(mat, n, m) {
    for (var i = 0; i < m; i++) {
	var mat_i = mat[i];
	for (var j = 0; j <= i + m; j++)
	    mat_i.a[j] = mat_i.a[j + m - i];
	for (; j <= m + m; j++)
	    mat_i.a[j] = 0;
    }
    var l = m;
    for (var k = 0; k < n; k++) {
	var mat_k = mat[k];
	mat_k.al = [];
	if (l < n) l++;
	var pivot_val = mat_k.a[0];
	if (Math.abs(pivot_val) < 1e-12) pivot_val = 1e-12;
	for (i = k + 1; i < l; i++) {
	    mat_i = mat[i];
	    var x = mat_i.a[0] / pivot_val;
	    mat_k.al.push(x);
	    for (j = 1; j <= m + m; j++)
		mat_i.a[j - 1] = mat_i.a[j] - x * mat_k.a[j];
	    mat_i.a[m + m] = 0;
	}
    }
}

function banbks(mat, v, n, m) {
    var l = m;
    for (var k = 0; k < n; k++) {
	var mat_k = mat[k];
	if (l < n) l++;
	for (var i = k + 1; i < l; i++)
	    v[i] -= mat_k.al[i - k - 1] * v[k];
    }
    l = 0;
    for (i = n - 1; i >= 0; i--) {
	var mat_i = mat[i];
	var x = v[i];
	for (k = 1; k <= l; k++)
	    x -= mat_i.a[k] * v[k + i];
	v[i] = x / mat_i.a[0];
	if (l < m + m) l++;
    }
}

// -- actual spiro code

function integ_spiro(k0, k1, k2, k3, n) {
    if (!n) n = 4;
    var th1 = k0;
    var th2 = .5 * k1;
    var th3 = (1./6) * k2;
    var th4 = (1./24) * k3;
    var x, y;
    var ds = 1. / n;
    var ds2 = ds * ds;
    var ds3 = ds2 * ds;
    var s = .5 * ds - .5;

    k0 *= ds;
    k1 *= ds;
    k2 *= ds;
    k3 *= ds;

    x = 0;
    y = 0;

    for (var i = 0; i < n; i++) {
	var u, v;
	var km0, km1, km2, km3;

	if (n == 1) {
	    km0 = k0;
	    km1 = k1 * ds;
	    km2 = k2 * ds2;
	} else {
	    km0 = (((1./6) * k3 * s + .5 * k2) * s + k1) * s + k0;
	    km1 = ((.5 * k3 * s + k2) * s + k1) * ds;
	    km2 = (k3 * s + k2) * ds2;
	}
	km3 = k3 * ds3;

	var t1_1 = km0;
	var t1_2 = .5 * km1;
	var t1_3 = (1./6) * km2;
	var t1_4 = (1./24) * km3;
	var t2_2 = t1_1 * t1_1;
	var t2_3 = 2 * (t1_1 * t1_2);
	var t2_4 = 2 * (t1_1 * t1_3) + t1_2 * t1_2;
	var t2_5 = 2 * (t1_1 * t1_4 + t1_2 * t1_3);
	var t2_6 = 2 * (t1_2 * t1_4) + t1_3 * t1_3;
	var t2_7 = 2 * (t1_3 * t1_4);
	var t2_8 = t1_4 * t1_4;
	var t3_4 = t2_2 * t1_2 + t2_3 * t1_1;
	var t3_6 = t2_2 * t1_4 + t2_3 * t1_3 + t2_4 * t1_2 + t2_5 * t1_1;
	var t3_8 = t2_4 * t1_4 + t2_5 * t1_3 + t2_6 * t1_2 + t2_7 * t1_1;
	var t3_10 = t2_6 * t1_4 + t2_7 * t1_3 + t2_8 * t1_2;
	var t4_4 = t2_2 * t2_2;
	var t4_5 = 2 * (t2_2 * t2_3);
	var t4_6 = 2 * (t2_2 * t2_4) + t2_3 * t2_3;
	var t4_7 = 2 * (t2_2 * t2_5 + t2_3 * t2_4);
	var t4_8 = 2 * (t2_2 * t2_6 + t2_3 * t2_5) + t2_4 * t2_4;
	var t4_9 = 2 * (t2_2 * t2_7 + t2_3 * t2_6 + t2_4 * t2_5);
	var t4_10 = 2 * (t2_2 * t2_8 + t2_3 * t2_7 + t2_4 * t2_6) + t2_5 * t2_5;
	var t5_6 = t4_4 * t1_2 + t4_5 * t1_1;
	var t5_8 = t4_4 * t1_4 + t4_5 * t1_3 + t4_6 * t1_2 + t4_7 * t1_1;
	var t5_10 = t4_6 * t1_4 + t4_7 * t1_3 + t4_8 * t1_2 + t4_9 * t1_1;
	var t6_6 = t4_4 * t2_2;
	var t6_7 = t4_4 * t2_3 + t4_5 * t2_2;
	var t6_8 = t4_4 * t2_4 + t4_5 * t2_3 + t4_6 * t2_2;
	var t6_9 = t4_4 * t2_5 + t4_5 * t2_4 + t4_6 * t2_3 + t4_7 * t2_2;
	var t6_10 = t4_4 * t2_6 + t4_5 * t2_5 + t4_6 * t2_4 + t4_7 * t2_3 + t4_8 * t2_2;
	var t7_8 = t6_6 * t1_2 + t6_7 * t1_1;
	var t7_10 = t6_6 * t1_4 + t6_7 * t1_3 + t6_8 * t1_2 + t6_9 * t1_1;
	var t8_8 = t6_6 * t2_2;
	var t8_9 = t6_6 * t2_3 + t6_7 * t2_2;
	var t8_10 = t6_6 * t2_4 + t6_7 * t2_3 + t6_8 * t2_2;
	var t9_10 = t8_8 * t1_2 + t8_9 * t1_1;
	var t10_10 = t8_8 * t2_2;
        u = 1;
        u -= (1./24) * t2_2 + (1./160) * t2_4 + (1./896) * t2_6 + (1./4608) * t2_8;
        u += (1./1920) * t4_4 + (1./10752) * t4_6 + (1./55296) * t4_8 + (1./270336) * t4_10;
        u -= (1./322560) * t6_6 + (1./1658880) * t6_8 + (1./8110080) * t6_10;
        u += (1./92897280) * t8_8 + (1./454164480) * t8_10;
        u -= 2.4464949595157930e-11 * t10_10;
        v = (1./12) * t1_2 + (1./80) * t1_4;
        v -= (1./480) * t3_4 + (1./2688) * t3_6 + (1./13824) * t3_8 + (1./67584) * t3_10;
        v += (1./53760) * t5_6 + (1./276480) * t5_8 + (1./1351680) * t5_10;
        v -= (1./11612160) * t7_8 + (1./56770560) * t7_10;
        v += 2.4464949595157932e-10 * t9_10;
	if (n == 1) {
	    x = u;
	    y = v;
	} else {
	    var th = (((th4 * s + th3) * s + th2) * s + th1) * s;
	    var cth = Math.cos(th);
	    var sth = Math.sin(th);

	    x += cth * u - sth * v;
	    y += cth * v + sth * u;
	    s += ds;
	}
    }
    return [x * ds, y * ds];
}

function seg_to_bez(ctx, ks, x0, y0, x1, y1) {
    var bend = Math.abs(ks[0]) + Math.abs(.5 * ks[1]) + Math.abs(.125 * ks[2]) + Math.abs((1./48) * ks[3]);

    if (bend < 1e-8) {
	ctx.lineTo(x1, y1);
    } else {
	var seg_ch = Math.sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0));
	var seg_th = Math.atan2(y1 - y0, x1 - x0);

	var xy = integ_spiro(ks[0], ks[1], ks[2], ks[3]);
	var ch = Math.sqrt(xy[0] * xy[0] + xy[1] * xy[1]);
	var th = Math.atan2(xy[1], xy[0]);
	var scale = seg_ch / ch;
	var rot = seg_th - th;
	if (bend < 1) {
	    var th_even = (1./384) * ks[3] + (1./8) * ks[1] + rot;
	    var th_odd = (1./48) * ks[2] + .5 * ks[0];
	    var scale3 = scale * (1./3);
	    var ul = scale3 * Math.cos(th_even - th_odd);
	    var vl = scale3 * Math.sin(th_even - th_odd);
	    var ur = scale3 * Math.cos(th_even + th_odd);
	    var vr = scale3 * Math.sin(th_even + th_odd);
	    ctx.bezierCurveTo(x0 + ul, y0 + vl, x1 - ur, y1 - vr, x1, y1);
	} else {
	    /* subdivide */
	    var ksub =
		[.5 * ks[0] - .125 * ks[1] + (1./64) * ks[2] - (1./768) * ks[3],
		 .25 * ks[1] - (1./16) * ks[2] + (1./128) * ks[3],
		 .125 * ks[2] - (1./32) * ks[3],
		 (1./16) * ks[3]
		 ];
	    var thsub = rot - .25 * ks[0] + (1./32) * ks[1] - (1./384) * ks[2] + (1./6144) * ks[3];
	    var cth = .5 * scale * Math.cos(thsub);
	    var sth = .5 * scale * Math.sin(thsub);
	    var xysub = integ_spiro(ksub[0], ksub[1], ksub[2], ksub[3]);
	    var xmid = x0 + cth * xysub[0] - sth * xysub[1];
	    var ymid = y0 + cth * xysub[1] + sth * xysub[0];
	    seg_to_bez(ctx, ksub, x0, y0, xmid, ymid);
	    ksub[0] += .25 * ks[1] + (1./384) * ks[3];
	    ksub[1] += .125 * ks[2];
	    ksub[2] += (1./16) * ks[3];
	    seg_to_bez(ctx, ksub, xmid, ymid, x1, y1);
	}
    }
}

function fit_euler(th0, th1) {
    var k1_old = 0;
    var error_old = th1 - th0;
    var k0 = th0 + th1;
    var k1 = 6 * (1 - Math.pow((.5 / Math.PI) * k0, 3)) * error_old;
    for (var i = 0; i < 10; i++) {
	var xy = integ_spiro(k0, k1, 0, 0);
        var error = (th1 - th0) - (.25 * k1 - 2 * Math.atan2(xy[1], xy[0]));
	if (Math.abs(error) < 1e-9) break;
	var new_k1 = k1 + (k1_old - k1) * error / (error - error_old);
	k1_old = k1;
	error_old = error;
	k1 = new_k1;
    }
    if (i == 10) { alert("fit_euler diverges at " + k0 + ", " + k1); }
    var chord = Math.sqrt(xy[0] * xy[0] + xy[1] * xy[1]);
    return {ks: [k0, k1], chord: chord};
}

function get_ths_straight() {
    return [0, 0];
}

function get_ths_left() {
    return [this.init_th1 + this.right.dth, this.init_th1 + this.right.dth];
}

function get_ths_right() {
    return [this.init_th0 - this.left.dth, this.init_th0 - this.left.dth];
}

function get_ths_g2() {
    return [this.init_th0 - this.left.dth, this.init_th1 + this.right.dth];
}

function setup_solver(path) {
    var segs = new Array;
    var nodes = new Array;

    for (var i = 0; i < path.length - 1; i++) {
	var seg = {};
	var dx = path[i + 1][0] - path[i][0];
	var dy = path[i + 1][1] - path[i][1];
	seg.th = Math.atan2(dy, dx);
	seg.chord = Math.sqrt(dx * dx + dy * dy);
	segs.push(seg);
    }
    for (i = 0; i < path.length; i++) {
	var node = {};
	node.xy = path[i];
	node.dth = 0;
	if (i > 0) {
	    node.left = segs[i - 1];
	}
	if (i < path.length - 1) {
	    node.right = segs[i];
	}
	if (node.left) node.left.right = node;
	if (node.right) node.right.left = node;
	if (node.left && node.right) {
	    var th = node.right.th - node.left.th;
	    if (th > Math.PI) th -= 2 * Math.PI;
	    if (th < -Math.PI) th += 2 * Math.PI;
	    node.th = th;
	    var chord_sum = node.left.chord + node.right.chord;
	    node.left.init_th1 = th * node.left.chord / chord_sum;
	    node.right.init_th0 = th * node.right.chord / chord_sum;
	}
	nodes.push(node);
    }
    for (var i = 0; i < segs.length; i++) {
        var seg = segs[i];
	if (seg.init_th0 == undefined) {
	    if (seg.init_th1 == undefined) {
		seg.init_th0 = 0;
		seg.init_th1 = 0;
		seg.get_ths = get_ths_straight;
	    } else {
		seg.init_th0 = seg.init_th1;
		seg.get_ths = get_ths_left;
	    }
	} else {
	    if (seg.init_th1 == undefined) {
		seg.init_th1 = seg.init_th0;
		seg.get_ths = get_ths_right;
	    } else {
		seg.get_ths = get_ths_g2;
	    }
	}
    }
    return {segs: segs, nodes: nodes};
}

function refine_euler(spline) {
    var segs = spline.segs;
    var nodes = spline.nodes;
    for (var i = 0; i < segs.length; i++) {
	var seg = segs[i];
	var ths = seg.get_ths();
	seg.params = fit_cloth(ths[0], ths[1]);
    }
    for (var i = 0; i < nodes.length; i++) {
	var node = nodes[i];
	if (node.left && node.right) {
	    var lparams = node.left.params;
	    kleft = (lparams.ks[0] + .5 * lparams.ks[1]) * lparams.chord / node.left.chord;
	    var rparams = node.right.params;
	    kright = (rparams.ks[0] - .5 * rparams.ks[1]) * rparams.chord / node.right.chord;
	    print('kleft = ' + String(kleft) + ', kright = ' + String(kright));
	}
    }
}

// some test framework to be deleted from production code
function eval_error(k0, k1, k2, k3) {
    if (k1 == undefined) k1 = 0;
    if (k2 == undefined) k2 = 0;
    if (k3 == undefined) k3 = 0;
    var xy = integ_spiro(k0, k1, k2, k3, 256);
    var errs = [0];
    for (var i = 1; i < 5; i++) {
	var my_xy = integ_spiro(k0, k1, k2, k3, i);
	var err = Math.sqrt((my_xy[0] - xy[0]) * (my_xy[0] - xy[0]) + (my_xy[1] - xy[1]) * (my_xy[1] - xy[1]));
        errs.push(err);
        var est_err = 4e-14 * Math.pow(k0, 12) +
	    4e-9 * Math.pow(k0, 2) * Math.pow(Math.abs(k1), 5) +
	    4e-10 * Math.pow(k1, 6) +
	    6e-10 * Math.pow(k2, 4) +
	    2e-10 * Math.pow(k3, 3);
	est_err = Math.pow(.08 * Math.abs(k0) + .2 * Math.sqrt(Math.abs(k1)) + .2 * Math.pow(Math.abs(k2), .333333) + .16 * Math.pow(Math.abs(k3), .25), 12);
	est_err = Math.pow(.006 * k0 * k0 + .03 * Math.abs(k1) + .03 * Math.pow(Math.abs(k2), .666667) + .025 * Math.pow(Math.abs(k3), .5), 6);
	est_err *= Math.pow(i, -12);
	print(String(i) + ': ' + String(err) + ', est ' + String(est_err));
    }
    print('err[1] / err[2] = ' + String(errs[1] / errs[2]) +
	  ', err[2] / err[4] = ' + String(errs[2] / errs[4]));
}

xp = [[10, 100], [50, 20], [100, 120], [150, 100]];
s = setup_solver(xp);
