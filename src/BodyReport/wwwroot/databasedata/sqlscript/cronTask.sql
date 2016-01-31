/*utf8*/
/*supprime les utilisateurs qui n'ont pas validé leur mail qui ont plus de 24 heures d'existence*/
delete from public."AspNetUsers" where "EmailConfirmed"=false and ("RegistrationDate" = null or (now()::timestamp - "RegistrationDate" > '24 hours') );