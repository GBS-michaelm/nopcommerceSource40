function SetImageModal(selector, modalSource, modalDiv) {

        $(selector).on('click', function () {
            $('.enlargeImageModalSource').attr('src', $(this).attr('src'));
            $('#enlargeImageModal').modal('show');

        });

}