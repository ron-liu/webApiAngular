angular.module 'app.leave', ['ui.router']

.config ['$stateProvider', ($stateProvider) ->
	$stateProvider
	.state 'home.dashboard',
		url: 'dashboard'
		templateUrl: 'app/dashboard.html'
		controller: [->]

	.state 'home.apply',
		url: 'apply'
		templateUrl: 'app/apply.html'
		controller: [->]

	.state 'home.list',
		url: 'list'
		templateUrl: 'app/list.html'
		controller: [->

		]
		resolve:
			leaves: ['LeaveService', (LeaveService) -> LeaveService.list() ]

	.state 'home.approve',
		url: 'approve'
		templateUrl: 'app/approve.html'
		controller: [->]
]

.factory 'LeaveService', [ 'Restangular', (Restangular)->
	list: ->Restangular.all('list').getList()
]
