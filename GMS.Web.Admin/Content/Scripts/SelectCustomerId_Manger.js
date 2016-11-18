$("#StaffID,#ModifyStaffID,#selectprov").select2({
    dropdownCssClass: "bigdrop"
});

function formatState(state) {
    //if (!state.id) { return state.text; }
    var $state = $(
      '<span>' + state.text + '</span>'
    );
    return $state;
};

var selectdCustId = $("#CustomerId").select2({
    placeholder: '--请选择--',
    dropdownCssClass: "bigdrop",
    escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
    templateResult: formatState,
    templateSelection: formatState,
    allowClear: true,
    ajax: {
        url: '/Business/GetCustomers',
        delay: 250,
        type: "POST",
        cache: true,
        data: function (params) {
            return {
                term: params.term
            };
        },
        processResults: function (data) {
            return {
                results: data
            };
        }
    }
});