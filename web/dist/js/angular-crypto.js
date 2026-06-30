angular.module('mdo-angular-cryptography', [])
    .provider('$crypto', function CryptoKeyProvider() {
        var cryptoKey;

        this.setCryptographyKey = function (value) {
            cryptoKey = value;
        };

        this.$get = [function () {
            return {
                getCryptoKey: function () {
                    return cryptoKey
                },

                encrypt: function (message, key) {

                   
                     
                    if (key === undefined) {
                        key = cryptoKey;
                    }
                    var newKey = CryptoJS.enc.Utf8.parse(key);
                    var iv = CryptoJS.enc.Utf8.parse(key);
                    return CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(message), newKey,
                        {
                            keySize: 128 / 8,
                            iv: iv,
                            mode: CryptoJS.mode.ECB,
                            padding: CryptoJS.pad.Pkcs7
                        }).toString();
                    // return CryptoJS.AES.encrypt(message, key, { mode: CryptoJS.mode.CBS, padding: CryptoJS.pad.Pkcs7 }).toString();
                },

                decrypt: function (message, key) {
                     
                    if (key === undefined) {
                        key = cryptoKey;
                    }
                    var newKey = CryptoJS.enc.Utf8.parse(key);
                    var iv = CryptoJS.enc.Utf8.parse(key);
                    var cipherBytes = CryptoJS.enc.Base64.parse(message);

                    return CryptoJS.AES.decrypt({ ciphertext: cipherBytes }, newKey,
                        {
                            keySize: 128 / 8,
                            iv: iv,
                            mode: CryptoJS.mode.ECB,
                            padding: CryptoJS.pad.Pkcs7
                        }).toString(CryptoJS.enc.Utf8);

                  //  return CryptoJS.AES.decrypt(message, key, { mode: CryptoJS.mode.ECB, padding: CryptoJS.pad.Pkcs7 }).toString(CryptoJS.enc.Utf8)
                }
            }
        }];
    });