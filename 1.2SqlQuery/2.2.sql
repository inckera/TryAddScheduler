-- 1.
select se.Surname Фамилия, se.Name Имя, sum(coalesce(sa.Quantity, 0)) "объем продаж" from Sellers se
left join Sales sa on sa.IDSel = se.ID
where sa.Date between '07.09.2013' and '07.10.2013'
group by se.ID, se.Surname, se.name
order by se.Surname, se.Name
 

-- 2.
 select p.Name "Продукт", se.Surname "Фамилия", se.Name "Имя"
,sum(coalesce(s.Quantity,0))* 100/(select  sum(coalesce(s1.Quantity,0)) from Products p1 
 left join sales s1 on s1.IDProd = p1.ID
 where p1.ID = p.ID and 
 p1.ID in (select a1.IDProd from Arrivals a1
 where a1.Date between '07.09.2013' and '07.10.2013'
 )group by p1.ID) "Процент"
 from Products p
inner join sales s on s.IDProd = p.ID
inner join Sellers se on se.ID = s.IDSel
where a.Date between '07.09.2013' and '07.10.2013'
and s.date between '07.09.2013' and '07.10.2013'
and p.ID in (select a.IDProd from Arrivals a
 where a1.Date between '07.09.2013' and '07.10.2013')
group by p.ID, p.Name, se.ID, se.Surname, se.Name
order by p.name, se.Surname, se.Name
