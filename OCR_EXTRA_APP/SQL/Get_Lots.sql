SELECT l.id_commune, l.id_bureau, l.id_lot,count(distinct id_acte) as nb_actes,l.status_lot
FROM Lot l INNER JOIN AffectationRegistre af on l.id_lot = af.id_lot
INNER JOIN Acte a on a.id_tome_registre = af.id_tome_registre
GROUP BY l.id_commune, l.id_bureau, l.id_lot,l.status_lot