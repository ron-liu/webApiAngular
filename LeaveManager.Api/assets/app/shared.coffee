angular.module 'app.shared', []

.directive 'submitValid', ['$parse', '$q', ($parse, $q) ->
	require: 'form'
	link: (scope, formElement, attributes, form) ->
		form.attempted = false
		formElement.bind 'submit', (event) ->
			scope.$apply  -> form.loading = true
			form.attempted = true
			if  not scope.$$phase then scope.$apply()
			fn = $parse attributes.submitValid
			fromForm = form.from ? form
			if (fromForm.$valid)
				scope.$apply  ->
					$q.when(fn scope, $event:event)
					.finally ->
						form.loading = false
						form.$setPristine()
						form.attempted = false
			else scope.$apply  -> form.loading = false
]