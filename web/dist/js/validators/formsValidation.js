/*
 *  Document   : formsValidation.js
 *  Author     : pixelcave
 *  Description: Custom javascript code used in Forms Validation page
 */
//document.write('<script language="javascript" src="../dist/js/validators/validationConfig.js"></script>');
var FormsValidation = function () {
    // document.write('<script language="javascript" src="../dist/js/validators/validationConfig.js"></script>');

    return {
        init: function (formName) {
            var listValiators = [];
            if (formName) {
                listValiators = ValidationConfig.Validators.filter(function (i, n) {
                    return i.formName == formName;
                });
            }
            else {
                listValiators = ValidationConfig.Validators;
            }
            for (var i = 0; i < listValiators.length; i++) {
                
                var validator = listValiators[i];
                $.validator.addMethod("valueNotEquals", function (value, element, arg) {
                    
                    
                    return arg !== value;
                }, "Value must not equal arg.");
                $.validator.addMethod("greaterThan", function (value, element,arg) {
                    
                    return value > arg;
                }, "Value must be greather than arg" );

                $('#' + listValiators[i].formName).validate({
                    errorClass: ValidationConfig.config.errorClass,
                    errorElement: ValidationConfig.config.errorElement,
                    errorPlacement: ValidationConfig.config.errorPlacement,
                    highlight: ValidationConfig.config.highlight,
                    success: ValidationConfig.config.success,
                    rules: listValiators[i].rules,
                    messages: listValiators[i].messages
                });
            }
            jQuery.validator.addMethod("pan", function (value, element) {
                return this.optional(element) || REGEX.PAN.test(value);
            }, "Please enter correct PAN");
            
            jQuery.validator.addMethod("gst", function (value, element) {
                
                return this.optional(element) || REGEX.GST.test(value);
            }, "Please enter correct GST");
            jQuery.validator.addMethod("prefix", function (value, element) {
                 
                return this.optional(element) || REGEX.BILL_PREFIX.test(value);
            }, "Letters, numbers, and underscores only please");
            jQuery.validator.addMethod("mobileIN", function (phone_number, element) {
                phone_number = phone_number.replace(/\(|\)|\s+|-/g, "");
                return this.optional(element) || phone_number.length > 9 &&
                    phone_number.match(REGEX.MobileIN);
            }, "Please specify a valid mobile number.");
            jQuery.validator.addMethod("notEqual", function (value, element, param) {
                return this.optional(element) || value != $(param).val();
            }, "This has to be different");
            /*
             *  Jquery Validation, Check out more examples and documentation at https://github.com/jzaefferer/jquery-validation
             */


            // Initialize Masked Inputs
            // a - Represents an alpha character (A-Z,a-z)
            // 9 - Represents a numeric character (0-9)
            // * - Represents an alphanumeric character (A-Z,a-z,0-9)
            // $('.maskednumber').mask('999.999');
            //$('#masked_date').mask('99/99/9999');
            //$('#masked_date2').mask('99-99-9999');
            //$('#masked_phone').mask('(999) 999-9999');
            //$('#masked_phone_ext').mask('(999) 999-9999? x99999');
            //$('#masked_taxid').mask('99-9999999');
            //$('#masked_ssn').mask('999-99-9999');
            //$('#masked_pkey').mask('a*-999-a999');
        }

        , intitWithConfig: function (formName, valConfig) {
            var listValiators = [];
            if (formName) {
                listValiators = valConfig.Validators.filter(function (i, n) {
                    return i.formName == formName;
                });
            }
            else {
                listValiators = valConfig.Validators;
            }
            for (var i = 0; i < listValiators.length; i++) {
                var validator = listValiators[i];
                $.validator.addMethod("valueNotEquals", function (value, element, arg) {
                    return arg !== value;
                }, "Value must not equal arg.");

                $('#' + listValiators[i].formName).validate({
                    errorClass: valConfig.config.errorClass,
                    errorElement: valConfig.config.errorElement,
                    errorPlacement: valConfig.config.errorPlacement,
                    highlight: valConfig.config.highlight,
                    success: valConfig.config.success,
                    rules: listValiators[i].rules,
                    messages: listValiators[i].messages
                });
            }
            jQuery.validator.addMethod("pan", function (value, element) {
                return this.optional(element) || REGEX.PAN.test(value);
            }, "Please enter correct PAN");
         
            jQuery.validator.addMethod("gst", function (value, element) {
                
                return this.optional(element) || REGEX.GST.test(value);
            }, "Please enter correct GST");

            jQuery.validator.addMethod("mobileIN", function (phone_number, element) {
                phone_number = phone_number.replace(/\(|\)|\s+|-/g, "");
                return this.optional(element) || phone_number.length > 9 &&
                    phone_number.match(REGEX.MobileIN);
            }, "Please specify a valid mobile number.");
            jQuery.validator.addMethod("prefix", function (value, element) {
                debugger
                return this.optional(element) || REGEX.BILL_PREFIX.test(value);
            }, "Letters, numbers, and underscores only please");
            /*
             *  Jquery Validation, Check out more examples and documentation at https://github.com/jzaefferer/jquery-validation
             */


            // Initialize Masked Inputs
            // a - Represents an alpha character (A-Z,a-z)
            // 9 - Represents a numeric character (0-9)
            // * - Represents an alphanumeric character (A-Z,a-z,0-9)
            // $('.maskednumber').mask('999.999');
            //$('#masked_date').mask('99/99/9999');
            //$('#masked_date2').mask('99-99-9999');
            //$('#masked_phone').mask('(999) 999-9999');
            //$('#masked_phone_ext').mask('(999) 999-9999? x99999');
            //$('#masked_taxid').mask('99-9999999');
            //$('#masked_ssn').mask('999-99-9999');
            //$('#masked_pkey').mask('a*-999-a999');
        }
    };
}();