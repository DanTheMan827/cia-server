const TILE_ORDER = [0, 1, 8, 9, 2, 3, 10, 11, 16, 17, 24, 25, 18, 19, 26, 27, 4, 5, 12, 13, 6, 7, 14, 15, 20, 21, 28, 29, 22, 23, 30, 31, 32, 33, 40, 41, 34, 35, 42, 43, 48, 49, 56, 57, 50, 51, 58, 59, 36, 37, 44, 45, 38, 39, 46, 47, 52, 53, 60, 61, 54, 55, 62, 63];

function _base64ToArrayBuffer(base64) {
    var binary_string = window.atob(base64);
    var len = binary_string.length;
    var bytes = new Uint8Array(len);
    for (var i = 0; i < len; i++) {
        bytes[i] = binary_string.charCodeAt(i);
    }
    return bytes.buffer;
}

function DrawIconToCanvas(iconData, canvasId, iconSize) {
	var canvas = document.getElementById(canvasId);
	canvas.height = iconSize;
	canvas.width = iconSize;

	var canvasCtx = canvas.getContext("2d");
	var i = 0;

	for (var tile_y = 0; tile_y < iconSize; tile_y += 8) {
		for (var tile_x = 0; tile_x < iconSize; tile_x += 8) {
			for (var k = 0; k < 8 * 8; k++) {
				var x = TILE_ORDER[k] & 0x7;
				var y = TILE_ORDER[k] >> 3;
				var color = iconData[i++] | (iconData[i++] << 8);

				var b = (color & 0x1f) << 3;
				var g = ((color >> 5) & 0x3f) << 2;
				var r = ((color >> 11) & 0x1f) << 3;

				canvasCtx.fillStyle = 'rgb(' + r + ',' + g + ',' + b + ')';
				canvasCtx.fillRect(x + tile_x, y + tile_y, 1, 1);
			}
		}
	}
}

function QrToCanvas(data, level, ppm, padding, backgroundColor, foregroundColor, canvasId) {
	var qrcode = new QrCode(data, [level]);
	var matrix = qrcode.getData();
	var paddingPixels = padding * ppm;
	var canvas = document.getElementById(canvasId);
	canvas.height = ppm * matrix.length + ((padding * 2) * ppm)
	canvas.width = canvas.height

	var context = this.ctx = canvas.getContext('2d')
	context.clearRect(0, 0, canvas.width, canvas.height)

	context.fillStyle = backgroundColor

	// draw left background padding
	context.fillRect(0, 0, paddingPixels, canvas.width)

	// draw top background padding
	context.fillRect(paddingPixels, 0, canvas.height - paddingPixels, paddingPixels)

	// draw right background padding
	context.fillRect(canvas.width - paddingPixels, paddingPixels, paddingPixels, canvas.height - (paddingPixels * 2))

	// draw bottom background padding
	context.fillRect(paddingPixels, canvas.height - paddingPixels, canvas.width - paddingPixels, paddingPixels)

	
	for (var x = 0; x < matrix.length; x++) {
		for (var y = 0; y < matrix.length; y++) {
			context.fillStyle = matrix[y][x] == 1 ? foregroundColor : backgroundColor
			context.fillRect(
				ppm * x + (padding * ppm),
				ppm * y + (padding * ppm),
				ppm, ppm
			)
		}
	}
}