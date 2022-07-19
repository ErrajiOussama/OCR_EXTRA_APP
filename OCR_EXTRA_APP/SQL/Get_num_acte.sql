SELECT a.id_acte,num_acte,imagepath
FROM AffectationRegistre af 
INNER JOIN Acte a on a.id_tome_registre = af.id_tome_registre 
where af.id_lot = @lots