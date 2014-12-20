angular.module 'app', ['ui.router', 'ui.bootstrap', 'app.security', 'app.config', 'app.leave', 'app.notification']

.config [ '$urlRouterProvider', '$stateProvider', 'RestangularProvider', 'AppConfig', ($urlRouterProvider, $stateProvider, RestangularProvider, AppConfig) ->

	RestangularProvider.setBaseUrl AppConfig.apiRootUrl

	$urlRouterProvider.otherwise '/'

	$stateProvider
	.state 'home',
		url: '/'
		abstract: true
		template: '<ui-view>'
		onEnter: ['$rootScope', '$window', 'NotificationManager', 'AuthDataStorage', ($rootScope, $window, NotificationManager, AuthDataStorage) ->
			$rootScope._ = $window._
			$rootScope.notifications = NotificationManager
			$rootScope.isSignedIn = AuthDataStorage.isSignedIn
			$rootScope.signOut = AuthDataStorage.remove
		]

	.state 'home.default',
		url: ''
		template: 'welcome'
		controller: [-> console.log 'fdsa']


]
