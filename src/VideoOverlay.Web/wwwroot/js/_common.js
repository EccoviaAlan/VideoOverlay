(function(Eccovia) {
	"use strict";

	function ensure(proto, props) {
		var names = Object.getOwnPropertyNames(props);
		for(var i = 0, len = names.length; i < len; i++) {
			if(!(names[i] in proto))
				proto[names[i]] = props[names[i]];
		}
	}

	//Static date functions
	var datePattern = /0?(\d{1,2})\/0?(\d{1,2})\/(\d{4}|\d{2})(?:\s+0?(\d{1,2})(?:\:0?(\d{1,2}))?(?:\:0?(\d{1,2})(\.\d+)?)?\s*(am|pm)?)?/i,
		sqlDate = /^([a-z]{3})[a-z]*\s+0?(\d\d?)\s+(\d\d(?:\d\d)?)(?:\s+0?(\d\d?)\:0?(\d\d?)(?:\:0?(\d{1,2})(\.\d+)?)?\s*(am|pm)?)?$/i,
		standardDateFormats = {
			d: "M/d/yyy",
			D: "dddd, MMMM d, yyyy",
			f: "dddd, MMMM d, yyyy h:mm tt",
			F: "dddd, MMMM d, yyyy h:mm:ss tt",
			g: "M/d/yyy h:mm tt",
			G: "M/d/yyy h:mm:ss tt",
			M: "MMMM d",
			O: "yyyy-MM-ddTHH:mm:ss.fffffff",
			s: "yyyy-MM-ddTHH:mm:ss",
			t: "h:mm tt",
			T: "h:mm:ss tt",
			Y: "MMMM, yyy"
		};
	standardDateFormats.m = standardDateFormats.M;
	standardDateFormats.o = standardDateFormats.O;
	standardDateFormats.y = standardDateFormats.Y;

	ensure(Date, {
		days: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
		shortDays: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
		months: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
		shortMonths: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
		standard: standardDateFormats,
		parseString: function(str) {
			if(str) {
				if(str instanceof Date)
					return str;
				var parts = str.match(datePattern);
				if(!parts && (parts = str.match(sqlDate)))
					parts[1] = Date.shortMonths.indexOf(parts[1]) + 1;
				if(parts) {
					var hr = parts[4],
						year = parseInt(parts[3], 10),
						month = (parseInt(parts[1]) || 1) - 1,	// In JS, month is 0-11 *shrug*
						day = parseInt(parts[2]),
						result;
					if(year < 100)
						year += 2000;
					if(hr) {
						hr = parseInt(hr) || 0;
						if(hr >= 0 && hr < 24) {
							if((parts[8] || "a").toLowerCase().charAt(0) === "p") {
								if(hr < 12)
									hr += 12;
							}
							else if(parts[8] && 12 === hr)
								hr = 0;
							result = new Date(year, month, day, hr, parseInt(parts[5]) || 0, parseInt(parts[6]) || 0, Math.round(1000 * (parseFloat(parts[7]) || 0)));
						}
					}
					if(!result)
						result = new Date(year, month, day);
					if(!isNaN(result) && result.getMonth() === month && result.getDate() === day)
						return result;
				}
				return NaN;
			}
			return new Date();
		},
		min: new Date(1753, 0, 1),
		max: new Date(9999, 11, 31),
		dateFormat: "d",
		dateTimeFormat: "g"
	});

	//Date object methods
	ensure(Date.prototype, {
		toFormat: function(format) {
			/// <summary>Formats the date using a .NET format string (see http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx)</summary>
			if(format === "R" || format === "r")
				return this.toUTCString();
			var d = this, temp;
			if(format === "u" || format === "U") {
				temp = new Date(this);
				temp.setMinutes(temp.getMinutes() + this.getTimezoneOffset());
				if(format === "u")
					return temp.toFormat("yyyy-MM-dd HH:mm:ssZ");
				return temp.toFormat("F");
			}
			if(format in Date.standard)
				format = Date.standard[format];
			return format.replace(/"[^"]*"|'[^']*'|(\w)\1*|0|#|\.|\,/g, function(match) {
				switch(match) {
					case "dd":
						if(d.getDate() < 10)
							return "0" + d.getDate();
					case "d":
						return d.getDate();
					case "ddd":
						return Date.shortDays[d.getDay()];
					case "dddd":
						return Date.days[d.getDay()];
					case "hh":
						temp = d.getHours() % 12;
						if(temp === 0)
							return "12";
						if(temp < 10)
							return "0" + temp;
						return temp;
					case "h":
						temp = d.getHours() % 12;
						if(temp === 0)
							return "12";
						return temp;
					case "HH":
						if(d.getHours() < 10)
							return "0" + d.getHours();
					case "H":
						return d.getHours();
					case "mm":
						if(d.getMinutes() < 10)
							return "0" + d.getMinutes();
					case "m":
						return d.getMinutes();
					case "MM":
						if(d.getMonth() < 9)
							return "0" + (d.getMonth() + 1);
					case "M":
						return d.getMonth() + 1;
					case "MMM":
						return Date.shortMonths[d.getMonth()];
					case "MMMM":
						return Date.months[d.getMonth()];
					case "ss":
						if(d.getSeconds() < 10)
							return "0" + d.getSeconds();
					case "s":
						return d.getSeconds();
					case "t":
						return d.getHours() >= 12 ? "P" : "A";
					case "tt":
						return d.getHours() >= 12 ? "PM" : "AM";
					case "yy":
						temp = d.getFullYear() % 100;
						if(temp < 10)
							return "0" + temp;
					case "y":
						return d.getFullYear() % 100;
					case "yyy":
						return d.getFullYear().toString().padLeft(3, "0");
					case "yyyy":
						return d.getFullYear().toString().padLeft(4, "0");
					case "yyyyy":
						return d.getFullYear().toString().padLeft(5, "0");
					case "K": //getTimezoneOffset is reversed +/-
						if(d.getTimezoneOffset() === 0)
							return "Z";
					case "zzz":
						return (-d.getTimezoneOffset() / 60) + ":" + (Math.abs(d.getTimezoneOffset()) % 60).toString().padLeft(2, "0");
					case "zz":
						if(d.getTimezoneOffset() < 600) {
							if(d.getTimezoneOffset() >= 0)
								return "-0" + (d.getTimezoneOffset() / 60);
							if(d.getTimezoneOffset() > -600)
								return "0" + (d.getTimezoneOffset() / -60);
						}
					case "z":
						return d.getTimezoneOffset() / -60;
				}
				switch(match.charAt(0)) {
					case "\"":
						return match.replace(/"/g, "");
					case "'":
						return match.replace(/'/g, "");
					case "\\":
						return match.charAt(1);
					case "f":
						return Math.floor(d.getMilliseconds() * Math.pow(10, match.length - 3)).toString().substr(0, match.length).padLeft(match.length, "0");
					case "F":
						return Math.floor(d.getMilliseconds() * Math.pow(10, match.length - 3)).toString().substr(0, match.length).padLeft(match.length, "0").replace(/0+$/, "");
					case "g":
						return d.getFullYear() > 0 ? "A.D." : "B.C.";
				}
				return match;
			});
		},
		toDateString: function() {
			return this.toFormat(Date.dateFormat);
		},
		toDateTimeString: function() {
			return this.toFormat(Date.dateTimeFormat);
		},
		addYears: function(years) {
			var n = new Date(this);
			n.setFullYear(n.getFullYear() + years);
			return n;
		},
		addMonths: function(months) {
			var n = new Date(this);
			n.setMonth(n.getMonth() + months);
			return n;
		},
		addDays: function(days) {
			var n = new Date(this);
			n.setDate(n.getDate() + days);
			return n;
		},
		addHours: function(hours) {
			var n = new Date(this);
			n.setHours(n.getHours() + hours);
			return n;
		},
		addMinutes: function(mins) {
			var n = new Date(this);
			n.setMinutes(n.getMinutes() + mins);
			return n;
		},
		addSeconds: function(secs) {
			var n = new Date(this);
			n.setSeconds(n.getSeconds() + secs);
			return n;
		},
		addMilliseconds: function(milli) {
			var n = new Date(this);
			n.setMilliseconds(n.getMilliseconds() + milli);
			return n;
		},
		dateOnly: function() {
			return new Date(this.getFullYear(), this.getMonth(), this.getDate());
		}
	});

	//Number static functions
	ensure(Number, {
		THOUSAND: ",",
		DECIMAL: ".",
		spell: function(num, decimals) {
			/// <summary>Spells out the number</summary>
			/// <param name="decimals">Bitwise whether or not to include decimals. 2 will add dollars and 4 will add cents (6 for both).
			var ones = ['', 'one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight', 'nine', 'ten', 'eleven', 'twelve', 'thirteen', 'fourteen', 'fifteen', 'sixteen', 'seventeen', 'eighteen', 'nineteen'],
				tens = ['', '', 'twenty', 'thirty', 'fourty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety'],
				sep = ['', ' thousand ', ' million ', ' billion ', ' trillion ', ' quadrillion ', ' quintillion ', ' sextillion ', " septillion ", " octillion ", " nonillion "],
				append = function(a) {
					var x = Math.floor(a / 100),
						y = Math.floor(a / 10) % 10,
						z = a % 10;

					return (x > 0 ? ones[x] + ' hundred ' : '') +
						(y >= 2 ? tens[y] + (ones[z] ? '-' + ones[z] : "") : ones[10 * y + z]);
				};
			Number.spell = function(num, decimals) {
				var n = parseFloat(num, 10),
					str = "";
				if(isNaN(n))
					return "";
				if(n < 0) {
					str = "negative ";
					n *= -1;
				}
				var oInt = Math.floor(n),
					val = oInt,
					arr = [],
					next;
				if(0 === oInt)
					str += "zero";
				else {
					while(val) {
						arr.push(val % 1000);
						val = Math.floor(val / 1000);
					}

					for(var i = 0, len = arr.length; i < len; i++) {
						if(next = append(arr[i]).trim())
							str = next + sep[i] + str;
					}
					str = str.trim();
				}
				if((decimals & 2) === 2)
					str += " dollars";
				if((decimals & 1) === 1) {
					str += " and ";
					str += n > oInt ? Math.floor((n - oInt) * 100) : "no";
					str += (decimals & 4) === 4 ? " cents" : "/100";
				}
				return str;
			};
			return Number.spell(num, decimals);
		}
	});

	//Number object methods
	ensure(Number.prototype, {
		toThousands: function(decimals) {
			var num = this.toFixed(decimals == undefined ? 2 : decimals);
			var start = num.indexOf('.');
			if(start == -1)
				start = num.length;
			if(Number.DECIMAL !== ".")
				num = num.replace(".", Number.DECIMAL);
			for(var i = start - 3; i > 0; i -= 3)
				if(num.charAt(i - 1) !== "-")
					num = num.substr(0, i) + Number.THOUSAND + num.substr(i);
			return num;
		},
		toFormat: function(format) {
			var parse = (/([cdefgnpxs])(\d\d?)?/).exec(format.toLowerCase());
			if(!parse)
				return this.toString();
			var precision = parse[2] ? parseInt(parse[2]) : undefined;
			switch(parse[1]) {
				case "c":
					return "$" + this.toThousands(precision);
				case "d":
					var num = parseInt(this);
					if(precision == undefined)
						return num.toString();
					var neg = num < 0;
					return neg ? "-" + (num * -1).toString().padLeft(precision, "0") : num.toString().padLeft(precision, "0");
				case "e":
					if(precision == undefined)
						return this.toExponential(6);
					return this.toExponential(precision);
				case "f":
					if(precision == undefined)
						return this.toFixed(2);
					return this.toFixed(precision);
				case "g":
					if(precision == undefined)
						return this.toPrecision();
					return this.toPrecision(precision);
				case "n":
					return this.toThousands(precision);
				case "p":
					return (this * 100).toThousands(precision) + " %";
				case "x":
					if(format.charAt(0) == "X") {
						if(precision == undefined)
							return this.toString(16).toUpperCase();
						return this.toString(16).toUpperCase().padLeft(precision, "0");
					}
					if(precision == undefined)
						return this.toString(16);
					return this.toString(16).padLeft(precision, "0");
				case "s":
					return Number.spell(this, precision);
			}
			return this.toString();
		}
	});

	//Static string functions
	ensure(String, {
		toInt: function(str) {
			if(!str || isNaN(str = parseInt(("" + str).replace(/^0+/g, ""), 10)))
				return 0;
			return str;
		},
		toFloat: function(str) {///<summary>Converts the string to a float, removing any non-number characters first</summary>
			if(!str)
				return 0;
			var value = parseFloat(str.toString().replace(/[^\d\-\.]/g, ""));
			if(isNaN(value))
				return 0;
			return value;
		}
	});

	//String object methods
	ensure(String.prototype, {
		trim: function() {///<summary>Removes leading and trailing whitespace</summary>
			return this.replace(/^\s+|\s+$/g, "");
		},
		trimStart: function() {///<summary>Removes leading whitespace</summary>
			return this.replace(/^\s+/g, "");
		},
		trimEnd: function() {///<summary>Removes trailing whitespace</summary>
			return this.replace(/\s+$/g, "");
		},
		padLeft: function(length, padWith) {
			if(this.length >= length)
				return this;
			if(!padWith && padWith !== 0)
				padWith = " ";
			var padding = "";
			for(var i = this.length; i < length; i++)
				padding += padWith;
			return padding.substr(0, length - this.length) + this;
		},
		padRight: function(length, padWith) {
			if(this.length >= length)
				return this;
			if(!padWith && padWith !== 0)
				padWith = " ";
			var padding = "";
			for(var i = this.length; i < length; i++)
				padding += padWith;
			return this + padding.substr(0, length - this.length);
		},
		toFormat: function(format) {
			if(format && this !== "") {
				if((/^\s*\$?\s*\-?\s*(\d[\d\,]*(\.\d+)?|\.\d+)\s*$/).test(this))
					return String.toFloat(this).toFormat(format);
				else {
					var date = new Date(this);
					if(!isNaN(date))
						return date.toFormat(format);
				}
			}
			return this;
		}
	});

	Eccovia.Filters = {
		date: function(value, format) {
			if(!value)
				return "";
			var d = new Date(value);
			if(d) {
				return d.toFormat(format || Date.dateFormat);
			}
			return "";
		}
	}
})(window.Eccovia || (Eccovia = {}));