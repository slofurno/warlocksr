"use strict";

var keyboard = (function () {
  var _keyboard = [];
  var keys = [
    ["left", 37],
    ["right", 39],
    ["up", 38],
    ["down", 40],
    ["jump", 32],
    ["rope", 68],
    ["shoot", 70],
    ["switch",82]
  ];

  window.onkeydown = function (e) {
    e.preventDefault();
    _keyboard[e.keyCode] = true;
    return false;
  };

  window.onkeyup = function (e) {
    e.preventDefault();
    _keyboard[e.keyCode] = false;
    return false;

  };

  var getState = function () {
    var state = {};
    keys.forEach(function (key) {
      state[key[0]] = (_keyboard[key[1]] === true);
    });
    return state;
  };

  return { getState: getState };

})();