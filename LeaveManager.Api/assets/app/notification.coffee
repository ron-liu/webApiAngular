angular.module 'app.notification', []

.config ['$httpProvider', ($httpProvider) ->
	$httpProvider.interceptors.push 'httpNotificationInterceptors'
]

.factory 'httpNotificationInterceptors', ['$q', 'NotificationManager', ($q, NotificationManager) ->
	request: (config) ->
		if config.method is 'POST' then NotificationManager.dismissAll()
		config
	responseError: (rejection) ->
		if rejection.status isnt 200 and rejection.status isnt 401 and rejection.status isnt 403
			console.log rejection
			NotificationManager.setMessages rejection.data.Messages
		$q.reject rejection
]

.factory 'NotificationManager', [ '$timeout','$rootScope', ($timeout, $rootScope) ->
	class NotificationManager
		constructor: ->	@messages = []

		dismissAll: => _.remove @messages

		setMessages: (messages) =>
			_.remove @messages

			_.each messages, (m, i)=>
				m.MessageType = switch m.MessageType
					when 'Error' then 'danger'
					when 'Warning' then 'warning'
					else 'success'
				$timeout ( => $rootScope.$apply => @messages.splice i, 1), 5000

			@messages = @messages.concat messages

	new NotificationManager()
]