importScripts('https://www.gstatic.com/firebasejs/9.22.0/firebase-app-compat.js');
importScripts('https://www.gstatic.com/firebasejs/9.22.0/firebase-messaging-compat.js');
firebase.initializeApp(
    {
        apiKey: "AIzaSyBXKrbcBBrq24chs44UlKmRGCH_lQ6L8Os",
        projectId: "oak-test-bfc55",
        messagingSenderId: "362923420806",
        appId: "1:362923420806:web:5204e48ad23c47f678b507",
    });
const fcm = firebase.messaging();
fcm.onBackgroundMessage(function(payload) {
    console.log('[firebase-messaging-sw.js] Received background message ', payload);
    //self.registration.showNotification("YOLO", {silent: true});
});