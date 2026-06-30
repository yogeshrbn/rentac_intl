var nav_initialized = false;
var nav_count = 0;
var nav_selected;
var nav_selectedIndex = -1;
var nav_Items = [];
var controller;

function nav_first() {
    // alert('nav_first');
    if (nav_count > 0) {
        nav_selectedIndex = 0;
        nav_selected = nav_Items[nav_selectedIndex];     
    } 
    disable();
}
function nav_previous() {
    // alert('nav_previous');
    if (nav_selectedIndex > 0) {
        nav_selectedIndex = nav_selectedIndex - 1;
        nav_selected = nav_Items[nav_selectedIndex];      
    }  
    disable();
}
function nav_next() {
    // alert('nav_next');
    if (nav_selectedIndex < nav_count - 1) {
        nav_selectedIndex = nav_selectedIndex + 1;
        nav_selected = nav_Items[nav_selectedIndex];
        
    }
    disable();
}
function nav_last() {
    // alert('nav_last');
    if (nav_count > 0) {
        nav_selectedIndex = nav_count - 1;
        nav_selected = nav_Items[nav_selectedIndex];
    }
    disable();
}
function disable() {
     
    var commands = '';
    enableToolbarCtl();
    if (nav_selectedIndex <= 0) {
        commands = 'first,previous';
    }
    if (nav_selectedIndex >= nav_Items.length - 1) {
        commands = 'next,last';
    }
    var cmds = commands.split(',');    
    $(cmds).each((e) => {
        $('#toolbarCtrls li[command="' + cmds[e] + '"]').addClass('disable-ctl');
    });    
}
function enableToolbarCtl() {
    $('#toolbarCtrls li[command !=undo]').removeClass('disable-ctl');
}