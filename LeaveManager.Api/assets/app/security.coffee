angular.module 'app.security', ['ui.router', 'LocalStorageModule', 'app.config', 'restangular', 'app.shared', 'ngMessages']

.config ['$httpProvider', ($httpProvider) -> $httpProvider.interceptors.push 'httpAuthInterceptors']

.config ['$stateProvider', ($StateProvider) ->
	$StateProvider
	.state 'home.signIn',
		url: 'sign-in'
		templateUrl : 'app/signIn.html'
		controller: ['$scope', 'SecurityService', ($scope, SecurityService)->
			$scope.submit = (model) -> SecurityService.signIn model
		]
	.state 'home.signUp',
		url: 'sign-up'
		templateUrl: 'app/signUp.html'
		controller: ['$scope', 'SecurityService', ($scope, SecurityService)->
			$scope.submit = (model) -> SecurityService.signUp model
		]

	.state 'home.resetPassword',
		url: 'resetPassword'
		templateUrl: 'app/reset-password'
		controller: [->

		]
]

.factory 'httpAuthInterceptors', ['ReturnUrlService', '$q', 'AppConfig', 'AuthDataStorage', (ReturnUrlService, $q, AppConfig, AuthDataStorage) ->
	request: (config) ->
		if config.url.indexOf(AppConfig.baseApiUrl) >=0
			config.headers['Authorization'] = "Bearer #{AuthDataStorage.getToken()}"
		config
	responseError: (rejection) ->
		switch rejection.status
			when 401, 403
				ReturnUrlService.relocateSignIn()
		$q.reject rejection
]

.factory 'ReturnUrlService', ['$injector', '$window', 'localStorageService', 'AppConfig', ($injector, $window, localStorageService, AppConfig) ->
	setDefaultNotSet: => localStorageService.add(AppConfig.returnUrlKey, localStorageService.get(AppConfig.returnUrlKey) ? 'home.default')
	relocateSignIn: =>
		currentName = $injector.get('$state').current.name
		localStorageService.add(AppConfig.returnUrlKey, currentName) # to avoid recursive dependency
		$injector.get('$state').go 'home.signIn'
	returnToUrl: =>
		url = localStorageService.get AppConfig.returnUrlKey
		localStorageService.remove AppConfig.returnUrlKey
		$injector.get('$state').go url ? 'home.default'
]

.factory 'SecurityService', ['Restangular', 'NotificationManager', '$timeout', '$state', 'AuthDataStorage', 'ReturnUrlService', (Restangular, NotificationManager, $timeout, $state, AuthDataStorage, ReturnUrlService) ->
	signIn: (model) ->
		data = "grant_type=password&username=" + model.Email + "&password=" + model.Password;
		Restangular.all('token').post data, {}, 'Content-Type': "application/x-www-form-urlencoded; charset=UTF-8"
		.then (res) ->
			AuthDataStorage.set res.access_token
			ReturnUrlService.returnToUrl()

	signUp: (model) ->
		Restangular.all('sign-up').post model
		.then (res) ->
			NotificationManager.setMessages [MessageType:'Info', Content:'User has been successfully created, will redirect to sign in page soon']
			$timeout (-> $state.go 'home.signIn'), 4000
]

.factory 'AuthDataStorage', ['localStorageService', 'AppConfig', (localStorageService, AppConfig) ->
	class AuthDataStorage
		constructor: ->
		_get: =>
			tokens = localStorageService.get(AppConfig.authDataKey)

		set: (token, userName) =>
			authData = _.extend @_get() ? {}, token: token, userName: userName
			localStorageService.add AppConfig.authDataKey, authData

		getToken: => @_get()?.token

		remove: => localStorageService.remove AppConfig.authDataKey

		isSignedIn: => @_get()?

	new AuthDataStorage()
]