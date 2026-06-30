var ValidationConfig = new function () {


    this.config = {
        errorClass: 'help-block animation-slideDown', // You can change the animation class for a different entrance animation - check animations page
        errorElement: 'div',
        errorPlacement: function (error, e) {
            // e.parents('.form-group > div').append(error);
            $(e).closest('div').append(error);
        },
        highlight: function (e) {

            $(e).parent().removeClass('has-success has-error').addClass('has-error');
            $(e).addClass('has-error');
            $(e).addClass('error');

            $(e).closest('.help-block').remove();
        },
        success: function (e) {

            // You can use the following if you would like to highlight with green color the input after successful validation!
            //  e.closest('.groupElement').removeClass('has-success has-error'); // e.closest('.form-group').removeClass('has-success has-error').addClass('has-success');
            //  $(e).removeClass('error').addClass('success');
            e.parent().removeClass('has-success has-error'); // e.closest('.form-group').removeClass('has-success has-error').addClass('has-success');

            e.closest('.help-block').remove();
        }
    };
    this.Validators = [];

    //login validators
    this.LoginValidators = {
        formName: 'form-login',
        rules: {
            loginName: {
                required: true

            },
            passwword: {
                required: true
            },
            ddlfinYear: {
                required: true,
                valueNotEquals: "0"
            },


        },
        messages: {
            loginName: {
                required: 'Please enter your login name.'
            },
            passwword: {
                required: 'Please enter your password.'
            },
            ddlfinYear: {
                valueNotEquals: 'Please select financial year.'
            }
        }
    };
    this.Validators.push(this.LoginValidators);
    //end of login validators

    this.CompanyValidators = {
        formName: 'form-company',
        rules: {
            val_contactPerson: {
                required: true,
                minlength: 3
            },
            val_companyName: {
                required: true
            },
            val_address1: {
                required: true
            },
            val_email: {
                required: true,
                email: true
            },

            val_website: {

                url: true
            },
            val_digits: {
                required: true,
                digits: true
            },
            val_number: {
                required: true,
                number: true
            },
            val_range: {
                required: true,
                range: [1, 1000]
            },
            val_terms: {
                required: true
            }
        },
        messages: {
            val_contactPerson: {
                required: 'Please enter a contact name'
                //minlength: 'Your username must consist of at least 3 characters'
            },
            val_companyName: {
                required: 'Please enter the company name'
            },
            val_address1: {
                required: 'Please enter the address1'
            },
            val_email: 'Please enter a valid email address',
            val_password: {
                required: 'Please provide a password',
                minlength: 'Your password must be at least 5 characters long'
            },
            val_confirm_password: {
                required: 'Please provide a password',
                minlength: 'Your password must be at least 5 characters long',
                equalTo: 'Please enter the same password as above'
            },
            val_bio: 'Don\'t be shy, share something with us :-)',
            val_skill: 'Please select a skill!',
            val_website: 'Please enter your website!',
            val_digits: 'Please enter only digits!',
            val_number: 'Please enter a number!',
            val_range: 'Please enter a number between 1 and 1000!',
            val_terms: 'You must agree to the service terms!'
        }
    };

    this.Validators.push(this.CompanyValidators);
    //---Client Validator form

    this.ClientValidators = {
        formName: 'form-client',
        rules: {
            val_contactPerson: {
                required: true,
                minlength: 3
            },
            val_clientName: {
                required: true
            },
            //val_address1: {
            //    required: true
            //},
            //val_email: {
            //    required: true,
            //    email: true
            //},
            val_clientTradeName: {
                required: true
            },
            //val_website: {

            //    url: true
            //},
            val_digits: {
                required: true,
                digits: true
            },
            val_number: {
                required: true,
                number: true
            },
            val_range: {
                required: true,
                range: [1, 1000]
            },
            val_terms: {
                required: true
            },
            val_ledgergroup: {
                required: true,
                valueNotEquals: '0'
            },
            val_Code: {
                required: true,
                minlength: 1,
                maxlength: 10
            }
        },
        messages: {
            val_contactPerson: {
                required: 'Please enter a contact name'
                //minlength: 'Your username must consist of at least 3 characters'
            },
            val_companyName: {
                required: 'Please enter the client name'
            },
            val_clientName: {
                required: 'Please enter the client name'
            },
            val_clientTradeName: {
                required: 'Please enter the client trade name'
            },
            //val_address1: {
            //    required: 'Please enter the address1'
            //},
            val_ledgergroup: {
                required: 'Please select group',
                valueNotEquals: 'Please select group'
            },
            val_Code: {
                required: 'Please enter unique code',
                minlength: 'Code must be minimum 1 and maximum of 10 characters long',
                maxlength: 'Code must be minimum 1 and maximum of 10 characters long'
            },
            //  val_email: 'Please enter a valid email address',
            //val_password: {
            //    required: 'Please provide a password',
            //    minlength: 'Your password must be at least 5 characters long'
            //},
            //val_confirm_password: {
            //    required: 'Please provide a password',
            //    minlength: 'Your password must be at least 5 characters long',
            //    equalTo: 'Please enter the same password as above'
            //},
            //val_bio: 'Don\'t be shy, share something with us :-)',
            //val_skill: 'Please select a skill!',
            ////    val_website: 'Please enter your website!',
            //val_digits: 'Please enter only digits!',
            //val_number: 'Please enter a number!',
            //val_range: 'Please enter a number between 1 and 1000!',
            //val_terms: 'You must agree to the service terms!',

        }
    };

    this.Validators.push(this.ClientValidators);

    //--end of client validators
    //--salt validator form
    this.SaltValidators = {
        formName: 'form-salt',
        rules: {
            val_saltName: {
                required: true,
                minlength: 3
            }
        },
        messages: {
            val_saltName: {
                required: 'Please enter salt name'
                //minlength: 'Your username must consist of at least 3 characters'
            }
        }
    };
    this.Validators.push(this.SaltValidators);

    this.LedgerValidators = {
        formName: 'form-ledger',
        rules: {
            val_ledgerName: { required: true, minlength: 10 }
            , val_ledgerCode: { required: true, minlength: 4 }
            , val_ledgerGroup: { required: true }
            , val_ledgeOpBal: { required: true }
            , val_ledgerDLN: { required: true }
            , val_ledgerDLExp: { required: true }
            , val_ledgerPAN: { required: true, minlength: 10 }
            , val_ledgerSTNo: { required: true }
            , val_ledgerSTExp: { required: true }


        },
        messages: {
            val_ledgerName: { required: 'Please enter ledger name' },
            val_ledgerCode: { required: 'Please enter ledger code' },
            val_ledgerGroup: { required: 'Please select ledger group' },
            val_ledgeOpBal: { required: 'Please enter opening balance' },
            val_ledgerDLN: { required: 'Please enter DL Number' },
            val_ledgerDLExp: { required: 'Please enter DL expiry date' },
            val_ledgerPAN: { required: 'Please enter PAN' },
            val_ledgerSTNo: { required: 'Please enter ST number' },
            val_ledgerSTExp: { required: 'Please enter ST expiry date' }

        }
    };
    this.Validators.push(this.LedgerValidators);
    this.LedgerContactValidtors = {
        formName: 'form-ledgerContact',
        rules: {
            val_ledgerAddr1: { required: true }
            , val_ledgerState: { required: true }
            , val_ledgerCity: { required: true }
            , val_ledgerZipCode: { required: true }
            , val_ledgerEmail: { required: true }
            , val_ledgeroffPhone: { required: true, minlength: 10 }
            , val_ledgerContactPerson: { required: true, minlength: 5 }
        },
        messages: {
            val_ledgerAddr1: { required: 'Please enter address1' },
            val_ledgerState: { required: 'Please select state' },
            val_ledgerCity: { required: 'Please select City' },
            val_ledgerZipCode: { required: 'Please select zip code' },
            val_ledgerEmail: { required: 'Please enter email' },
            val_ledgeroffPhone: { required: 'Please enter office phone' },
            val_ledgerContactPerson: { required: 'Please enter contact person name' }
        }
    };
    this.Validators.push(this.LedgerContactValidtors);
    //--product validator form
    this.ProductValidators = {
        formName: 'form-product',
        rules: {
            val_productName: {
                required: true,
                minlength: 3
            },
            val_productCode: {
                required: true,
                minlength: 1
            },
            val_productUnit: {
                required: true,
                minlength: 3
            },
            //val_productPacking: {
            //    required: true,

            //},
            val_hsnCode: {
                 
                minlength: 4,
                maxlength: 8,
                number: true

            },
            val_productCategory: {
                required: true,
                minlenght: 3
            },
            val_localTax: {
                required: true,

            },
            val_productVat: {
                required: true,

            },

            val_cstRate: {
                required: true,

            },
            val_mrp: {
                required: true,

            },
            val_purchaseRate: {
                required: true,

            },
            val_packingQty: {
                required: true,
            }

        },
        messages: {
            val_productName: {
                required: 'Please enter product name'
            },
            val_productCode: {
                required: 'Please enter product code'
            },
            val_productUnit: {
                required: 'Please enter product unit'
            },
            //val_productPacking: {
            //    required: 'Please enter product description'
            //},
            val_productCategory: {
                required: 'Please enter product category'
            },
            val_localTax: {
                required: 'Please enter local tax%'
            },
            val_productVat: {
                required: 'Please enter VAT%'
            },

            val_cstRate: {
                required: 'Please enter CST Rate'
            },
            val_mrp: {
                required: 'Please enter mrp'
            },
            val_purchaseRate: {
                required: 'Please enter purchase rate'
            },
            val_packingQty: {
                required: 'Please enter packing quantity'
            },
            val_hsnCode: {
                
                minlength: 'HSN Code can be 4 to 8 digits long',
                maxlength: 'HSN Code can be  4 to 8 digits long',

                number: 'HSN Code must be a number'


            }
        }
    };
    this.Validators.push(this.ProductValidators);
    //----------------- Work Order Validators----------------------------

    this.WorkOrderValidators = {
        formName: 'form-workorder',
        rules: {
            client_value: {
                required: true,
                min: 1
            },
            val_Site: {
                required: true,
                min: 1
            },
            //val_Vehicle: {
            //    required: true,
            //    min: 1
            //},
            //val_Driver: {
            //    required: true,
            //    min: 1
            //},
            val_workOrderDate: {
                required: true,
                minlength: 3
            },
            val_workOrderNumber: {
               
                prefix: true

            },
            val_clientAmount: {
                required: true

            },
            company_value: {
                required: true

            }


        },
        messages: {
            client_value: {
                required: 'Please select client',
                min: 'Please select client'
            },
            val_Site: {
                required: 'Please enter the site',
                min: 'Please enter the site'
            },
            //val_Vehicle: {
            //    required: 'Please select vehicle',
            //    min: 'Please select vehicle'
            //},
            //val_Driver: {
            //    required: 'Please select driver',
            //    min: 'Please select driver'
            //},
            val_workOrderDate: {
                required: 'Please enter work order date'
            },
            val_workOrderNumber: {
                required: 'Please enter challan number',
                prefix: 'Challan number does not allow special characters except (/ and -)'

            },
            val_clientAmount: {
                required: 'Please enter amount'
            },
            company_value: {
                required: 'Please select company'
            }
        }
    };
    this.Validators.push(this.WorkOrderValidators);

    //-------end of work order validators--------------------------------
    //----------------- Work SiteValidators----------------------------

    this.SiteValidators = {
        formName: 'form-site',
        rules: {
            job_number: {
                required: true

            },
            challan_number: {
                required: true
            },
            shaft_size: {
                required: true
            },
            shaft_height: {
                required: true
            },
            site_name: {
                required: true

            }


        },
        messages: {
            job_number: {
                required: 'Please select job number'
            },
            challan_number: {
                required: 'Please enter challan number'
            },
            shaft_size: {
                required: 'Please enter shaft size'
            },
            shaft_height: {
                required: 'Please enter shaft height'
            },
            site_name: {
                required: 'Please enter site name'
            }
        }
    };
    this.Validators.push(this.SiteValidators);

    //-------end of work order Site validators--------------------------------


    //--receipt validators 

    this.ReceiptValidators = {
        formName: 'form-receipt',
        rules: {
            Amount: {
                required: true,
                min: 1

            },
            Ledger: {
                required: true,
                valueNotEquals: "0"
            },
            site: {
                required: false
            },
            txn_mode: {
                required: true,
                valueNotEquals: "0"
            },
            //Remarks: {
            //    required: true
            //},
            TransactionDate: {
                required: true
            },
            val_refedger: {
                required: true,
                valueNotEquals: "0"
            }


        },
        messages: {
            Amount: {
                required: 'Please enter amount greator than 0.',
                min: 'Please enter amount greator than 0.'
            },
            txn_mode: {
                required: 'Please select trasaction mode',
                valueNotEquals: 'Please select trasaction mode'
            },
            Ledger: {
                required: 'Please select party.',
                valueNotEquals: 'Please select party.'
            },
            //Remarks: {
            //    required: 'Please enter description.'
            //},
            TransactionDate: {
                required: 'Please select transaction date.'
            },
            val_refedger: {
                required: 'Please select deposit to account.',
                valueNotEquals: 'Please select deposit to account.'
            }
        }
    };
    this.Validators.push(this.ReceiptValidators);

    // end of receipt validators
    this.BillingValidator = {
        formName: 'form-bill',
        rules: {
            party: {
                required: true,
                valueNotEquals: "0"
            },
            site: {
                required: true,
                valueNotEquals: "0"
            },
            txtFrom: { required: true },
            txtTo: { required: true },


        },
        messages: {

            party: {
                required: 'Please select the party.',
                valueNotEquals: 'Please select the party'
            },
            site: {
                required: 'Please select the site.',
                valueNotEquals: 'Please select the site'
            },
            txtFrom: {
                required: 'Please select the from date.'
            }, txtTo: {
                required: 'Please select to date.'
            }
        }
    };
    this.Validators.push(this.BillingValidator);

    this.MaterialAdjust = {
        formName: 'form-mat',
        rules: {
            client: {
                required: true,
                valueNotEquals: "0"
            },
            site: {
                required: true,
                valueNotEquals: "0"
            },
            txtFrom: { required: true },
            txtTo: { required: true },


        },
        messages: {

            client: {
                valueNotEquals: 'Please select the party.'
            },
            site: {
                valueNotEquals: 'Please select the site.'
            },
            txtFrom: {
                required: 'Please select the from date.'
            }, txtTo: {
                required: 'Please select to date.'
            }
        }
    };
    this.Validators.push(this.MaterialAdjust);
    //add edit empplyee
    this.addEditEmployeeValidator = {
        formName: 'form-employee',
        rules: {
            val_empName: {
                required: true,
                minlength: 3
            },
            val_empCode: {
                required: true
            },
            val_address: {
                required: true
            },
            val_aadhar: {
                required: true,
                minlength: 3
            },
            val_phone: {
                required: true,
                minlength: 3
            }
        },
        messages: {
            val_empName: {
                required: 'Employee name is required.'
            },
            val_empCode: {
                required: 'Employee code is required.'
            },
            val_address: {
                required: 'Address is required.'
            },
            val_aadhar: {
                required: 'Aadahar is required.'
            },
            val_phone: {
                required: 'Phone is required.'
            }
        }
    };
    this.Validators.push(this.addEditEmployeeValidator);
    //add/edit client site
    this.clientSiteValidator = {
        formName: 'form-clientsite',
        rules: {
            address: {
                required: true,
                minlength: 3
            },
            //contactPerson: {
            //    required: true
            //},
            //phone: {
            //    required: true
            //},
            //city: {
            //    required: true,
            //    minlength: 3
            //},
            //state: {
            //    required: true,
            //    minlength: 3
            //}
        },
        messages: {
            project: {
                required: 'Project  is required.'
            },
            address: {
                required: 'Address  is required.'
            },
            //contactPerson: {
            //    required: 'Contact person is required.'
            //},
            //phone: {
            //    required: 'Phone is required.'
            //},
            //city: {
            //    required: 'City is required.'
            //},
            //state: {
            //    required: 'State is required.'
            //}
        }
    };
    this.Validators.push(this.clientSiteValidator);

    //vehicle
    this.vehicleValidators = {
        formName: 'form-vehicle',
        rules: {
            name: {
                required: true,
                minlength: 3
            },
            regNumber: {
                required: true
            },


        },
        messages: {
            address: {
                required: 'Vehicle name  is required.'
            },
            regNumber: {
                required: 'Registration number is required.'
            },


        }
    };
    this.Validators.push(this.vehicleValidators);
    this.RbnClientSettignsValidators = {
        formName: 'frm-rbnclientprofileSettings',
        rules: {
            val_rbnStateId: {
                required: true
            },
            val_address1: {
                required: true
            },
            val_city: {
                required: true
            },
            val_pinCode: {
                required: true
            },
            val_email: {
                required: true,
                email: true
            },
            val_mobile: {
                required: true,
                mobileIN: true
            },
            val_spocName: {
                required: true
            },
            val_gst: {

                gst: true
            },
            val_pan: {
                //minlength: 10,
                //maxlength: 10,
                pan: true
            }
        },
        messages: {
            val_rbnStateId: {
                required: 'State is required.'
            },
            val_address1: {
                required: 'Address1 is required.'
            },
            val_city: {
                required: 'City is required.'
            },
            val_pinCode: {
                required: 'Pincode is required.'
            },
            val_gst: {
                required: 'Enter a valid GST no.'
            },
            val_pan: {
                required: 'Enter a valid PAN.'
            },
            val_email: {
                required: 'Email is required',
                email: 'Pleae enter a valid email'
            },
            val_mobile: {
                required: 'Mobile is required'
            },
            val_spocName: {
                required: 'Spoc name is required'
            },
        }
    };

    this.Validators.push(this.RbnClientSettignsValidators);
    // register client form
    this.rbnClientRegisterValidator = {
        formName: 'form-signup',
        rules: {
            val_name: {
                required: true
            },
            val_email: {
                required: true,
                email: true
            },
            val_mobile: {
                required: true,
                mobileIN: true,
                minlength: 8,
                maxlength: 20
            },
            val_company: {
                required: true
            },

        },
        messages: {
            val_name: {
                required: 'Fullname is required.'
            },
            val_email: {
                required: 'Email is required.',
                email: 'Enter valid email address'
            },
            val_mobile: {
                required: 'Mobile is required.',
                mobileIN: 'Enter valid mobile number',
                minlength: 'Enter valid mobile number',
                maxlength: 'Enter valid mobile number'
            },
            val_company: {
                required: 'Company is required.'
            }

        }
    };

    this.Validators.push(this.rbnClientRegisterValidator);


    // register client form
    this.grnValidator = {
        formName: 'form-grn',
        rules: {
            val_party: {
                required: true,
                valueNotEquals: "0"
            },
            val_site: {
                required: true,
                valueNotEquals: "0"
            },
            val_receivingDate: {
                required: true
            },
            //val_sender: {
            //    required: true
            //},


        },
        messages: {
            val_party: {
                required: 'Fullname is required.',
                valueNotEquals: 'Party is required'
            },
            val_site: {
                required: 'Email is required.',
                valueNotEquals: 'Site is required'
            },
            val_receivingDate: {
                required: 'Mobile is required.'
            },
            val_sender: {
                required: 'Sender is required.'
            },

        }
    };

    this.Validators.push(this.grnValidator);
    this.grnAddLineItemValidator = {
        formName: 'form-grn-lineItem',
        rules: {
            val_product: {
                required: true,
                valueNotEquals: "0"
            },
            val_qty: {
                required: true,
                valueNotEquals: "0"
            },
            val_receivingDate: {
                required: true
            },
            val_sender: {
                required: true
            },


        },
        messages: {
            val_party: {
                required: 'Fullname is required.',
                valueNotEquals: 'Party is required'
            },
            val_site: {
                required: 'Email is required.',
                valueNotEquals: 'Site is required'
            },
            val_receivingDate: {
                required: 'Mobile is required.'
            },
            val_sender: {
                required: 'Sender is required.'
            },

        }
    };

    this.Validators.push(this.grnAddLineItemValidator);

    this.matTransferFormValidators = {
        formName: 'form-matTransfer',
        rules: {
            val_party: {
                required: true,
                greaterThan: 0
            },
            val_site: {
                required: true,
                greaterThan: 0
            },
            val_receivingDate: {
                required: true
            }


        },
        messages: {
            val_party: {
                required: 'Party is required.',
                greaterThan: 'Party is required'
            },
            val_site: {
                required: 'Site is required.',
                greaterThan: 'Site is required'
            },
            val_receivingDate: {
                required: 'Date is required.'
            }


        }
    };

    this.Validators.push(this.matTransferFormValidators);

    this.frmStockAdjustValidators = {
        formName: 'frmStockAdjust',
        rules: {
            //val_party: {
            //    required: true,
            //    greaterThan: 0
            //},
            val_refNumber: {
                required: true,
                greaterThan: 0
            },
            val_receivingDate: {
                required: true
            },
            val_reason: {
                required: true,

            }

        },
        messages: {
            //val_party: {
            //    required: 'Party is required.',
            //    greaterThan: 'Party is required'
            //},
            val_refNumber: {
                required: 'Ref. No is required.',
                greaterThan: 'Site is required'
            },
            val_receivingDate: {
                required: 'Date is required.'
            },
            val_reason: {
                required: 'Reason is required.'
            }

        }
    };

    this.Validators.push(this.frmStockAdjustValidators);
    this.frmStockAdjustListValidators = {
        formName: 'frmStockAdjustList',
        rules: {

            val_fromDate: {
                required: true
            },
            val_toDate: {
                required: true,

            }

        },
        messages: {

            val_fromDate: {
                required: 'From Date is required.'
            },
            val_toDate: {
                required: 'To Date is required.'
            }

        }
    };

    this.Validators.push(this.frmStockAdjustListValidators);
    this.frmChangePwdValidators = {
        formName: 'frmChangePwd',
        rules: {

            val_currentPassword: {
                required: true
            },
            val_newPassword: {
                // required: true,
                minlength: 8,
                maxlength: 20,
                notEqual: '[name="val_currentPassword"]'
            },
            val_confirmPassword: {
                equalTo: '[name="val_newPassword"]'

            }

        },
        messages: {

            val_currentPassword: {
                required: 'Current password required.'
            },
            val_newPassword: {
                // required: 'New password is required.',
                minlength: 'Password must be between 8 to 20 characters',
                minlength: 'Password must be between 8 to 20 characters',
                notEqual: 'New password must not be equal to the current password'
            },
            val_confirmPassword: {
                required: 'Confirm password is required.'
            }

        }
    };

    this.Validators.push(this.frmChangePwdValidators);
    this.frmBillConfigValidators = {
        formName: 'frmBillConfig',
        rules: {
            val_preFix: {
                required: true,
                minlength: 3,
                maxlength: 20,

            },
            val_billNo: {
                required: true,
                minlength: 1,
                maxlength: 6,
                number: true
            },
        },
        messages: {
            val_preFix: {
                required: 'Prefix is required.',

            },
            val_billNo: {
                required: 'Start from is required.',
                minlength: 'Start from must be between 1 to 10000',
                minlength: 'Start from must be between 1 to 10000',
                number: 'Start from must be a number'
            }


        }
    };

    this.Validators.push(this.frmBillConfigValidators);
    this.frmForgotPwdValidators = {
        formName: 'frmForgotPwd',
        rules: {

            val_email: {
                required: true,
                email: true
            },


        },
        messages: {


            val_email: {
                required: 'Email address is required',
                email: 'Enter a valid email address'
            }


        }
    };

    this.Validators.push(this.frmForgotPwdValidators);
    this.frmForgotResetpwdValidators = {
        formName: 'frmForgotResetPwd',
        rules: {


            val_password: {
                required: true,
                minlength: 8,
                maxlength: 20

            },
            val_confirmPassword: {
                equalTo: '[name="val_password"]'

            }
        },
        messages: {


            val_password: {
                required: 'Password is required',
                minlength: 'Password must be between 8 to 20 characters',
                minlength: 'Password must be between 8 to 20 characters',

            },
            val_confirmPassword: {
                required: 'Confirm password is required.'
            }

        }
    };

    this.Validators.push(this.frmForgotResetpwdValidators);
    this.frmPurchaseValidators = {
        formName: 'frmPurchase',
        rules: {


            val_party: {
                required: true,
                min: 1
            },
            val_partyBillDate: {
                required: true,
                minlength: 3

            },
            val_partyBillNo: {
                required: true,
                minlength: 1
            }

        },
        messages: {


            val_party: {
                required: 'Please select a vendor',
                min: 'Please select a vendor'

            },
            val_partyBillDate: {
                required: 'Bill date is required.'
            },
            val_partyBillNo: {
                required: 'Party bill no is required.'
            }

        }
    };

    this.Validators.push(this.frmPurchaseValidators);
    this.frmQuotationValidators = {
        formName: 'frmQuotation',
        rules: {


            val_party: {
                required: function () {
                    var v = $('input[name="quotation_party_type"]').val();
                    var pt = parseInt(v, 10);
                    if (isNaN(pt)) pt = 1;
                    return pt === 1;
                },
                min: {
                    param: 1,
                    depends: function () {
                        var v = $('input[name="quotation_party_type"]').val();
                        var pt = parseInt(v, 10);
                        if (isNaN(pt)) pt = 1;
                        return pt === 1;
                    }
                }
            },
            val_unregistered_party_name: {
                required: function () {
                    var v = $('input[name="quotation_party_type"]').val();
                    var pt = parseInt(v, 10);
                    if (isNaN(pt)) pt = 1;
                    return pt === 2;
                }
            },
            val_unregistered_party_address: {
                required: function () {
                    var v = $('input[name="quotation_party_type"]').val();
                    var pt = parseInt(v, 10);
                    if (isNaN(pt)) pt = 1;
                    return pt === 2;
                }
            },
            val_partyBillDate: {
                required: true,


            },
            val_partyBillNo: {
                required: true,

            }

        },
        messages: {


            val_party: {
                required: 'Please select a vendor',
                min: 'Please select a vendor'

            },
            val_unregistered_party_name: {
                required: 'Party name is required.'
            },
            val_unregistered_party_address: {
                required: 'Party address is required.'
            },
            val_partyBillDate: {
                required: 'QuotationDate date is required.'
            },
            val_partyBillNo: {
                required: 'Quotation number is required.'
            }

        }
    };

    this.Validators.push(this.frmQuotationValidators);

    this.frmTransporterValidators = {
        formName: 'frmTransporter',
        rules: {


            val_name: {
                required: true,
                required: 10
            },
            val_gst: {
                required: true,
                minlength: 15

            },
            val_email: {
                email: true
            },
            val_phone: {
                mobileIN: true
            }


        },
        messages: {


            val_name: {
                required: 'Please enter name',
                required: 'Name must be a minimum of 10 characters long'

            },
            val_gst: {
                required: 'Please enter a valid GST',
                minlength: 'Please enter a valid GST'
            },
            val_email: {
                email: 'Please enter a valid Email',
            },
            val_phone: {
                mobileIN: 'Please enter a valid Mobile No',
            },
        }
    };
    this.Validators.push(this.frmTransporterValidators);

    this.frmEwayBill_addVehhicle = {
        formName: 'frmewaybill_addvehhicle',
        rules: {


            val_fromplace: {
                required: true,

            },
            val_fromstate: {
                required: true,

            },
            val_reasoncode: {
                required: true,

            },
            val_reasonremarks: {
                required: true,

            },
            val_vehicleno: {
                required: true,
                minlength: 10
            },
            val_transMode: {
                required: true,

            },
            val_vehicleType: {
                required: true,

            }

        },
        messages: {

            val_fromplace: {
                required: 'Please select transporter',

            },
            val_fromstate: {
                required: 'From state is required',
            },
            val_reasoncode: {
                required: 'Reason is required',
            },
            val_reasonremarks: {
                required: 'Remarks is required',
            },
            val_vehicleno: {
                required: 'Vehicle No is required',
                minlength: 'Vehicle No is required',
            },
            val_vehicleType: {
                required: 'Vehicle type is required',
            },
            val_transMode: {
                required: 'Please select transportation mode',
            },
        }
    };
    this.Validators.push(this.frmEwayBill_addVehhicle);


    this.frmCancelEWayBill = {
        formName: 'frmcancelewaybill',
        rules: {


            val_reasonCode: {
                required: true,
                min: 1
            },
            val_reasonremarks: {
                required: true,
                minlength: 10,
                maxlength: 50
            }

        },
        messages: {

            val_reasonCode: {
                required: 'Please select reason of cancellation',

            },
            val_reasonremarks: {
                required: 'Please enter the remarks',
                minlenght: 'Remarks must be a minimum of 10 characters long',
                maxlength: 'Remarks can be a maximum of 50 characters long'


            },
        }
    };
    this.Validators.push(this.frmCancelEWayBill);
    this.frmContractValidators = {
        formName: 'frmContract',
        rules: {

            val_title: {
                required: true,

            },
            val_party: {
                required: true,
                min: 1
            },
            val_site: {
                required: true,

            },
            val_contractType: {
                required: true,
                min: 1
            },
            val_effectiveFrom: {
                required: true,

            },
            val_duration: {
                required: true,
                min: 1
            },
            val_area: {
                required: true,
                min: 1
            },


        },
        messages: {
            val_title: {
                required: 'Title is required',

            },
            val_party: {
                required: 'Party is required',
                min: 'Party is required'
            },
            val_site: {
                required: 'Site is required',

            },
            val_contractType: {
                required: 'Contract Type is required',

            },
            val_effectiveFrom: {
                required: 'Effective from is required',

            },
            val_duration: {
                required: 'Duration is required',

            },
            val_area: {
                required: 'Area is required',

            },
        }
    };
    this.Validators.push(this.frmContractValidators);

    this.frmContractBillValidators = {
        formName: 'frmContractBill',
        rules: {

            val_amount: {
                required: true,
                min: 1
            },
            val_remarks: {
                required: true,
                minlength: 10
            },



        },
        messages: {
            val_amount: {
                required: 'Bill amount is required',
                minlength: 'Bill amount must be a positive amount',
            },
            val_remarks: {
                required: 'Remarks is required',
                minlength: 'Please enter minimum 10 characters'
            },

        }
    };
    this.Validators.push(this.frmContractBillValidators);


    this.frmMatLossBillValidators = {
        formName: 'frmLossEntry',
        rules: {

            val_party: {
                required: true,
                min: 1
            },
            val_site: {
                required: true,
                min: 1
            },
            val_entryDate: {
                required: true,

            },



        },
        messages: {
            val_party: {
                required: 'Party is required',
                min: 'Bill amount must be a positive amount',
            },
            val_site: {
                required: 'Site is required',
                min: 'Site is required'
            },
            val_entryDate: {
                required: 'Entry date is required',

            },
        }
    };
    this.Validators.push(this.frmMatLossBillValidators);

    this.frmWorkStationType = {
        formName: 'frmWorkStationType',
        rules: {

            type_name: {
                required: true,
                maxlength: 50
            }



        },
        messages: {
            type_name: {
                required: 'Workstation type is required',
                maxlength: 'Maximum 50 characters are allowed',
            },

        }
    };
    this.Validators.push(this.frmWorkStationType);

    this.frmWorkStation = {
        formName: 'frmWorkStation',
        rules: {
            name: {
                required: true,
                maxlength: 50

            },
            type_id: {
                required: true,
                min: 1
            }



        },
        messages: {
            name: {
                required: 'Workstation name is required',
                maxlength: 'Maximum 50 characters are allowed',
            },
            type_id: {
                required: 'Workstation type is required',
                min: 'Workstation type is required'
            }

        }
    };
    this.Validators.push(this.frmWorkStation);

    this.frmOperation = {
        formName: 'frmOperation',
        rules: {
            name: {
                required: true,
                maxlength: 50

            }




        },
        messages: {
            name: {
                required: 'Operation name is required',
                maxlength: 'Maximum 50 characters are allowed',
            }


        }
    };
    this.Validators.push(this.frmOperation);

    this.fromWoJobCard = {
        formName: 'fromWoJobCard',
        rules: {
            jobNumber: {
                required: true,
                maxlength: 20
            },
            days: {
                required: true,
                maxlength: 3
            },
            quantity: {
                required: true,
                maxlength: 3
            },
            employee: {
                required: true,

            },
            clientContactName: {
                required: true,
                maxlength: 30
            },
            clientContactMobile: {
                required: true,

            },
            estimatedStartDate: {
                required: true,

            }




        },
        messages: {
            jobNumber: {
                required: 'Job number is required',
                maxlength: 'Job number can be 20 characters long',
            },
            days: {
                required: 'Days is required',
                maxlength: 'Maximum days can be 999'
            },
            quantity: {
                required: 'Quantity is required',
                maxlength: 'Maximum quantity can be 999'
            },
            employee: {
                required: 'Employee is required',

            },
            clientContactName: {
                required: 'Client contact person is required for this job',
                maxlength: 'Contact name can be 30 characters long'
            },
            clientContactMobile: {
                required: 'Contact number is required',
                maxlength: 'Maximum length of number can be 10'

            },
            estimatedStartDate: {
                required: 'Please enter a valid esitmated start date in dd/MM/yyyy format'

            }
        }
    };
    this.Validators.push(this.fromWoJobCard);

    this.frmExtendConract = {
        formName: 'frmExtendConract',
        rules: {

            val_extRemarks: {
                required: true,
                maxlength: 500
            },

            val_extendDate: {
                required: true,

            }




        },
        messages: {
            val_extRemarks: {
                required: 'Please enter remarks',
                maxlength: 'Remarks can not be more then 500 letter'

            }
            ,
            val_extendDate: {
                required: 'Please enter a valid date in dd/MM/yyyy format'

            }
        }
    };
    this.Validators.push(this.frmExtendConract);


    this.frmEmailSetup = {
        formName: 'frmEmailSetup',
        rules: {
            smtp_server: {
                required: true,
                maxlength: 50
            },
            smtp_port: {
                required: true,
                maxlength: 5
            },
            smtp_username: {
                required: true,
                maxlength: 50
            },
            smtp_password: {
                required: true,
                maxlength: 50
            },
            fromEmailAddress: {
                required: true,
                maxlength: 50
            }





        },
        messages: {
            smtp_server: {
                required: 'SMTP Server is required',
                maxlength: 'Maximum 50 characters are allwed'
            },
            smtp_port: {
                required: 'SMTP Server Port is required',
                maxlength: 'Maximum 5 numbers are allwed'
            },
            smtp_username: {
                required: 'SMTP user name is required',
                maxlength: 'Maximum 50 characters are allwed'
            },
            smtp_password: {
                required: 'SMTP password is required',
                maxlength: 'Maximum 50 characters are allwed'

            },
            fromEmailAddress: {
                required: 'From email address is required',
                maxlength: 'Maximum 50 characters are allwed'
            }
        }
    };
    this.Validators.push(this.frmEmailSetup);

    this.frmSendEmailForm = {
        formName: 'frmSendEmailForm',
        rules: {
            receipients: {
                required: true,
                maxlength: 500
            },
            copyTo: {

                maxlength: 500
            },
            subject: {
                required: true,
                maxlength: 100
            },
            emailBody: {
                required: true,
                maxlength: 8000
            }





        },
        messages: {
            receipients: {
                required: 'Please enter the receipient',
                maxlength: 'Only 500 characters are allowed'
            },
            copyTo: {

                maxlength: 'Only 500 characters are allowed'
            },
            subject: {
                required: 'Email subject is required',
                maxlength: 'Only 500 characters are allowed'
            },
            emailBody: {
                required: 'Email body is required',
                maxlength: 'Only 8000 characters are allowed'

            }
        }
    };
    this.Validators.push(this.frmSendEmailForm);

    this.frmUser = {
        formName: 'frmUser',
        rules: {
            full_name: {
                required: true,
                maxlength: 50
            },
            email: {
                maxlength: 50
            },
            phone: {
                required: true,
                maxlength: 10
            },
            password: {
                required: true,
                maxlength: 20
            },
            confirmPassword: {
                required: true,
                maxlength: 20
            }
        },
        messages: {
            full_name: {
                required: 'Please enter the full name of the user',
                maxlength: 'Only 50 characters are allowed'
            },
            email: {

                maxlength: 'Only 50 characters are allowed'
            },
            phone: {
                required: 'Please enter a valid phone number',
                maxlength: 'Only 10 characters are allowed'
            },
            password: {
                required: 'Passwor is required',
                maxlength: 'Only 20 characters are allowed for the password'

            },
            confirmPassword: {
                required: 'Confirm password is reqired',
                maxlength: 'Only 20 characters are allowed for the password'

            }
        }
    };
    this.Validators.push(this.frmUser);

    this.frmZone = {
        formName: 'frmZone',
        rules: {
            name: {
                required: true,
                maxlength: 50
            }
           
        },
        messages: {
            name: {
                required: 'Please enter the zone name',
                maxlength: 'Only 50 characters are allowed'
            },
          
             
        }
    };
    this.Validators.push(this.frmZone);
}();