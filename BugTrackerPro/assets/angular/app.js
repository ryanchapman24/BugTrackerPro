var monarchApp = angular.module('monarchApp', ['ngRoute', 'ngAnimate']);

// configure our routes
monarchApp.config(function($routeProvider) {
    $routeProvider

    .when('/', {
        templateUrl : 'pages/index.html',
        controller  : 'indexController'
    })
    .when('/index', {
        templateUrl : 'pages/index.html',
        controller  : 'indexController'
    })
        .when ('/advanced-datatables', {
        templateUrl : 'pages/advanced-datatables.html',
        controller  : 'advanced-datatablesController'
    })

        .when ('/animations', {
        templateUrl : 'pages/animations.html',
        controller  : 'animationsController'
    })

        .when ('/bs-carousel', {
        templateUrl : 'pages/bs-carousel.html',
        controller  : 'bs-carouselController'
    })

        .when ('/buttons', {
        templateUrl : 'pages/buttons.html',
        controller  : 'buttonsController'
    })

        .when ('/calendar', {
        templateUrl : 'pages/calendar.html',
        controller  : 'calendarController'
    })

        .when ('/chart-boxes', {
        templateUrl : 'pages/chart-boxes.html',
        controller  : 'chart-boxesController'
    })

        .when ('/chart-js', {
        templateUrl : 'pages/chart-js.html',
        controller  : 'chart-jsController'
    })

        .when ('/chat', {
        templateUrl : 'pages/chat.html',
        controller  : 'chatController'
    })

        .when ('/checklist', {
        templateUrl : 'pages/checklist.html',
        controller  : 'checklistController'
    })

        .when ('/ckeditor', {
        templateUrl : 'pages/ckeditor.html',
        controller  : 'ckeditorController'
    })

        .when ('/collapsable', {
        templateUrl : 'pages/collapsable.html',
        controller  : 'collapsableController'
    })

        .when ('/content-boxes', {
        templateUrl : 'pages/content-boxes.html',
        controller  : 'content-boxesController'
    })

        .when ('/data-tables', {
        templateUrl : 'pages/data-tables.html',
        controller  : 'data-tablesController'
    })

        .when ('/dropzone-uploader', {
        templateUrl : 'pages/dropzone-uploader.html',
        controller  : 'dropzone-uploaderController'
    })

        .when ('/fixed-datatables', {
        templateUrl : 'pages/fixed-datatables.html',
        controller  : 'fixed-datatablesController'
    })

        .when ('/flot-charts', {
        templateUrl : 'pages/flot-charts.html',
        controller  : 'flot-chartsController'
    })

        .when ('/forms-elements', {
        templateUrl : 'pages/forms-elements.html',
        controller  : 'forms-elementsController'
    })

        .when ('/forms-masks', {
        templateUrl : 'pages/forms-masks.html',
        controller  : 'forms-masksController'
    })

        .when ('/forms-validation', {
        templateUrl : 'pages/forms-validation.html',
        controller  : 'forms-validationController'
    })

        .when ('/forms-wizard', {
        templateUrl : 'pages/forms-wizard.html',
        controller  : 'forms-wizardController'
    })

        .when ('/gmaps', {
        templateUrl : 'pages/gmaps.html',
        controller  : 'gmapsController'
    })

        .when ('/helper-classes', {
        templateUrl : 'pages/helper-classes.html',
        controller  : 'helper-classesController'
    })

        .when ('/icons', {
        templateUrl : 'pages/icons.html',
        controller  : 'iconsController'
    })

        .when ('/image-crop', {
        templateUrl : 'pages/image-crop.html',
        controller  : 'image-cropController'
    })

        .when ('/images', {
        templateUrl : 'pages/images.html',
        controller  : 'imagesController'
    })

        .when ('/index-alt', {
        templateUrl : 'pages/index-alt.html',
        controller  : 'index-altController'
    })

        .when ('/inline-editor', {
        templateUrl : 'pages/inline-editor.html',
        controller  : 'inline-editorController'
    })

        .when ('/input-knobs', {
        templateUrl : 'pages/input-knobs.html',
        controller  : 'input-knobsController'
    })

        .when ('/just-gage', {
        templateUrl : 'pages/just-gage.html',
        controller  : 'just-gageController'
    })

        .when ('/labels-badges', {
        templateUrl : 'pages/labels-badges.html',
        controller  : 'labels-badgesController'
    })

        .when ('/lazyload', {
        templateUrl : 'pages/lazyload.html',
        controller  : 'lazyloadController'
    })

        .when ('/loading-feedback', {
        templateUrl : 'pages/loading-feedback.html',
        controller  : 'loading-feedbackController'
    })

        .when ('/mailbox-compose', {
        templateUrl : 'pages/mailbox-compose.html',
        controller  : 'mailbox-composeController'
    })

        .when ('/mailbox-inbox', {
        templateUrl : 'pages/mailbox-inbox.html',
        controller  : 'mailbox-inboxController'
    })

        .when ('/mailbox-single', {
        templateUrl : 'pages/mailbox-single.html',
        controller  : 'mailbox-singleController'
    })

        .when ('/mapael', {
        templateUrl : 'pages/mapael.html',
        controller  : 'mapaelController'
    })

        .when ('/markdown', {
        templateUrl : 'pages/markdown.html',
        controller  : 'markdownController'
    })

        .when ('/modals', {
        templateUrl : 'pages/modals.html',
        controller  : 'modalsController'
    })

        .when ('/morris-charts', {
        templateUrl : 'pages/morris-charts.html',
        controller  : 'morris-chartsController'
    })

        .when ('/multi-uploader', {
        templateUrl : 'pages/multi-uploader.html',
        controller  : 'multi-uploaderController'
    })

        .when ('/nav-menus', {
        templateUrl : 'pages/nav-menus.html',
        controller  : 'nav-menusController'
    })

        .when ('/notifications', {
        templateUrl : 'pages/notifications.html',
        controller  : 'notificationsController'
    })

        .when ('/page-transitions', {
        templateUrl : 'pages/page-transitions.html',
        controller  : 'page-transitionsController'
    })

        .when ('/panel-boxes', {
        templateUrl : 'pages/panel-boxes.html',
        controller  : 'panel-boxesController'
    })

        .when ('/pickers', {
        templateUrl : 'pages/pickers.html',
        controller  : 'pickersController'
    })

        .when ('/pie-gages', {
        templateUrl : 'pages/pie-gages.html',
        controller  : 'pie-gagesController'
    })

        .when ('/popovers-tooltips', {
        templateUrl : 'pages/popovers-tooltips.html',
        controller  : 'popovers-tooltipsController'
    })

        .when ('/progress-bars', {
        templateUrl : 'pages/progress-bars.html',
        controller  : 'progress-barsController'
    })

        .when ('/response-messages', {
        templateUrl : 'pages/response-messages.html',
        controller  : 'response-messagesController'
    })

        .when ('/responsive-datatables', {
        templateUrl : 'pages/responsive-datatables.html',
        controller  : 'responsive-datatablesController'
    })

        .when ('/responsive-tables', {
        templateUrl : 'pages/responsive-tables.html',
        controller  : 'responsive-tablesController'
    })

        .when ('/scrollbars', {
        templateUrl : 'pages/scrollbars.html',
        controller  : 'scrollbarsController'
    })

        .when ('/sliders', {
        templateUrl : 'pages/sliders.html',
        controller  : 'slidersController'
    })

        .when ('/social-boxes', {
        templateUrl : 'pages/social-boxes.html',
        controller  : 'social-boxesController'
    })

        .when ('/sortable-elements', {
        templateUrl : 'pages/sortable-elements.html',
        controller  : 'sortable-elementsController'
    })

        .when ('/sparklines', {
        templateUrl : 'pages/sparklines.html',
        controller  : 'sparklinesController'
    })

        .when ('/summernote', {
        templateUrl : 'pages/summernote.html',
        controller  : 'summernoteController'
    })

        .when ('/tables', {
        templateUrl : 'pages/tables.html',
        controller  : 'tablesController'
    })

        .when ('/tabs', {
        templateUrl : 'pages/tabs.html',
        controller  : 'tabsController'
    })

        .when ('/tile-boxes', {
        templateUrl : 'pages/tile-boxes.html',
        controller  : 'tile-boxesController'
    })

        .when ('/timeline', {
        templateUrl : 'pages/timeline.html',
        controller  : 'timelineController'
    })

        .when ('/vector-maps', {
        templateUrl : 'pages/vector-maps.html',
        controller  : 'vector-mapsController'
    })

        .when ('/xcharts', {
        templateUrl : 'pages/xcharts.html',
        controller  : 'xchartsController'
    })

    .when('/admin-blog', {
        templateUrl : 'pages/admin-blog.html',
        controller  : 'admin-blogController'
    })

    .when('/admin-pricing', {
        templateUrl : 'pages/admin-pricing.html',
        controller  : 'admin-pricingController'
    })

    .when('/auto-menu', {
        templateUrl : 'pages/auto-menu.html',
            controller  : 'auto-menuController'
    })

    .when('/faq-section', {
        templateUrl : 'pages/faq-section.html',
            controller  : 'faq-sectionController'
    })

    .when('/invoice', {
        templateUrl : 'pages/invoice.html',
        controller  : 'invoiceController'
    })

    .when('/portfolio-gallery', {
        templateUrl : 'pages/portfolio-gallery.html',
        controller  : 'portfolio-galleryController'
    })

    .when('/portfolio-masonry', {
        templateUrl : 'pages/portfolio-masonry.html',
        controller  : 'portfolio-masonryController'
    })

    .when('/slidebars', {
        templateUrl : 'pages/slidebars.html',
        controller  : 'slidebarsController'
    })

    .when('/view-profile', {
        templateUrl : 'pages/view-profile.html',
        controller  : 'view-profileController'
    })

});


monarchApp.controller('indexController', function($scope) {
    
});

monarchApp.controller('advanced-datatablesController', function($scope) {
    
});

monarchApp.controller('animationsController', function($scope) {
    
});

monarchApp.controller('bs-carouselController', function($scope) {
    
});

monarchApp.controller('buttonsController', function($scope) {
    
});

monarchApp.controller('calendarController', function($scope) {
    
});

monarchApp.controller('chart-boxesController', function($scope) {
    
});

monarchApp.controller('chart-jsController', function($scope) {
    
});

monarchApp.controller('chatController', function($scope) {
    
});

monarchApp.controller('checklistController', function($scope) {
    
});

monarchApp.controller('ckeditorController', function($scope) {
    
});

monarchApp.controller('collapsableController', function($scope) {
    
});

monarchApp.controller('content-boxesController', function($scope) {
    
});

monarchApp.controller('data-tablesController', function($scope) {
    
});

monarchApp.controller('dropzone-uploaderController', function($scope) {
    
});

monarchApp.controller('fixed-datatablesController', function($scope) {
    
});

monarchApp.controller('flot-chartsController', function($scope) {
    
});

monarchApp.controller('forms-elementsController', function($scope) {
    
});

monarchApp.controller('forms-masksController', function($scope) {
    
});

monarchApp.controller('forms-validationController', function($scope) {
    
});

monarchApp.controller('forms-wizardController', function($scope) {
    
});

monarchApp.controller('gmapsController', function($scope) {
    
});

monarchApp.controller('helper-classesController', function($scope) {
    
});

monarchApp.controller('iconsController', function($scope) {
    
});

monarchApp.controller('image-cropController', function($scope) {
    
});

monarchApp.controller('imagesController', function($scope) {
    
});

monarchApp.controller('indexController', function($scope) {
    
});

monarchApp.controller('index-altController', function($scope) {
    
});

monarchApp.controller('inline-editorController', function($scope) {
    
});

monarchApp.controller('input-knobsController', function($scope) {
    
});

monarchApp.controller('just-gageController', function($scope) {
    
});

monarchApp.controller('labels-badgesController', function($scope) {
    
});

monarchApp.controller('lazyloadController', function($scope) {
    
});

monarchApp.controller('loading-feedbackController', function($scope) {
    
});

monarchApp.controller('mailbox-composeController', function($scope) {
    
});

monarchApp.controller('mailbox-inboxController', function($scope) {
    
});

monarchApp.controller('mailbox-singleController', function($scope) {
    
});

monarchApp.controller('mapaelController', function($scope) {
    
});

monarchApp.controller('markdownController', function($scope) {
    
});

monarchApp.controller('modalsController', function($scope) {
    
});

monarchApp.controller('morris-chartsController', function($scope) {
    
});

monarchApp.controller('multi-uploaderController', function($scope) {
    
});

monarchApp.controller('nav-menusController', function($scope) {
    
});

monarchApp.controller('notificationsController', function($scope) {
    
});

monarchApp.controller('page-transitionsController', function($scope) {
    
});

monarchApp.controller('panel-boxesController', function($scope) {
    
});

monarchApp.controller('pickersController', function($scope) {
    
});

monarchApp.controller('pie-gagesController', function($scope) {
    
});

monarchApp.controller('popovers-tooltipsController', function($scope) {
    
});

monarchApp.controller('progress-barsController', function($scope) {
    
});

monarchApp.controller('response-messagesController', function($scope) {
    
});

monarchApp.controller('responsive-datatablesController', function($scope) {
    
});

monarchApp.controller('responsive-tablesController', function($scope) {
    
});

monarchApp.controller('scrollbarsController', function($scope) {
    
});

monarchApp.controller('slidersController', function($scope) {
    
});

monarchApp.controller('social-boxesController', function($scope) {
    
});

monarchApp.controller('sortable-elementsController', function($scope) {
    
});

monarchApp.controller('sparklinesController', function($scope) {
    
});

monarchApp.controller('summernoteController', function($scope) {
    
});

monarchApp.controller('tablesController', function($scope) {
    
});

monarchApp.controller('tabsController', function($scope) {
    
});

monarchApp.controller('tile-boxesController', function($scope) {
    
});

monarchApp.controller('timelineController', function($scope) {
    
});

monarchApp.controller('vector-mapsController', function($scope) {
    
});

monarchApp.controller('xchartsController', function($scope) {
    
});

monarchApp.controller('admin-blogController', function($scope){

});

monarchApp.controller('admin-pricingController', function($scope){

});

monarchApp.controller('auto-menuController', function($scope){

});

monarchApp.controller('faq-sectionController', function($scope){

});

monarchApp.controller('invoiceController', function($scope){

});

monarchApp.controller('portfolio-galleryController', function($scope){

});

monarchApp.controller('portfolio-masonryController', function($scope){

});

monarchApp.controller('slidebarsController', function($scope){

});

monarchApp.controller('view-profileController', function($scope){

});

