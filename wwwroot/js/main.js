"use strict";

/*var root = document.getElementById("root");
var table = document.createElement("table");
for (var y = 0; y < 24; y++) {
    var tr = document.createElement("tr");
    for (var x = 31; x >= 0; x--) {
        var td = document.createElement("td");
        td.id = "pixel_".concat(y * 32 + x);
        tr.appendChild(td);
    }
    table.appendChild(tr);
}

root.appendChild(table);*/

var connection = new signalR.HubConnectionBuilder().withUrl("/cameraHub").build();

connection.on("ReceiveFrame", function (frame) {

    let min = Number(frame.min);
    let max = Number(frame.max);

    if (max - 1 < min) {
        max++;
    }

    const autoTemp = document.getElementById("tempAuto");

    if (autoTemp.checked == false) {
	min = document.getElementById("tempMin").value;
	max = document.getElementById("tempMax").value;
    }

const canvas = document.getElementById('canvas');
	const ctx = canvas.getContext('2d');

	const newPixels = [];

    frame.pixels.forEach(function (pixel, idx) {

        const temp = Number(pixel);
        const normalised = Math.floor(255 * (temp - min) / (max - min));
        const bgColor = "rgb(" + normalised.toString() + ", 0, " + (255 - normalised).toString() + ")";

	    newPixels.push(normalised);
	    newPixels.push(0);
	    newPixels.push(255-normalised);
	    newPixels.push(255);
	});	
	     const imgData = new ImageData( new Uint8ClampedArray( newPixels ), 32, 24 );
	     ctx.putImageData( imgData, 0, 0 );
	     ctx.globalCompositeOperation = "copy";
	     ctx.scale( 20, 20 ); 

	    // newest browsers can control interpolation quality
	     ctx.imageSmoothingQuality = "high";
	     ctx.drawImage( canvas, 0, 0 );
	    //
	    // clean
	     ctx.globalCompositeOperation = "source-over";
	     ctx.setTransform( 1, 0, 0, 1, 0, 0);
});
connection.start()["catch"](function (err) {
    return console.error(err.toString());
});
